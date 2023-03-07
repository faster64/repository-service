using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Caching;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.DomainEventProcessWorker.Exceptions;
using KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.Events;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient.Dtos;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Microsoft.EntityFrameworkCore;
using Polly;
using ServiceStack;
using ServiceStack.Configuration;

namespace KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.EventHandler
{
    public class PayslipPaymentIntegrationEventHandler :
        IIntegrationEventHandler<VoidedPayslipPaymentIntegrationEvent>,
        IIntegrationEventHandler<CreatedPayslipPaymentIntegrationEvent>
    {
        private EfDbContext _db;
        private readonly IKiotVietApiClient _kiotVietApiClient;
        private readonly ICacheClient _cacheClient;
        private ILogger<PayslipPaymentIntegrationEventHandler> _logger;
        private ILogger<PayslipPaymentIntegrationEventHandler> Logger => _logger ?? (_logger = HostContext.Resolve<ILogger<PayslipPaymentIntegrationEventHandler>>());
        private readonly IAppSettings _appSettings;
        private readonly Helper _helper = new Helper();
        public PayslipPaymentIntegrationEventHandler(EfDbContext db, IKiotVietApiClient kiotVietApiClient, IAppSettings appSettings, ICacheClient cacheClient)
        {
            _db = db;
            _kiotVietApiClient = kiotVietApiClient;
            _appSettings = appSettings;
            _cacheClient = cacheClient;
        }

        public async Task Handle(VoidedPayslipPaymentIntegrationEvent @event)
        {
            await UpdatePayslipTotalPaymentAsync(@event.PayslipId, @event.Context);
        }

        public async Task Handle(CreatedPayslipPaymentIntegrationEvent @event)
        {
            await UpdatePayslipTotalPaymentAsync(@event.PayslipId, @event.Context);
        }

        private async Task UpdatePayslipTotalPaymentAsync(long payslipId, IntegrationEventContext context)
        {
            var policy = Policy
                .Handle<KvTimeSheetPayslipNullException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (ex, time) =>
                    {
                        Logger.LogWarning($@"Retry change amount of payslip id {payslipId} after {time.TotalSeconds}s");
                    });

            await policy.ExecuteAsync(async () =>
            {
                using (_db = await _helper.GetDbContextByGroupId(context.GroupId,
                    context.RetailerCode, _cacheClient, _kiotVietApiClient, _appSettings))
                {
                    var payslip = await _db.Payslips.AsNoTracking().FirstOrDefaultAsync(ps => ps.Id == payslipId);
                    if (payslip != null)
                    {

                        var result = await _kiotVietApiClient.GetTotalPaymentByPayslipId(
                            new GetTotalPaymentByPayslipIdsReq
                            {
                                RetailerId = payslip.TenantId,
                                PayslipIds = new List<long> { payslipId }
                            }, context.GroupId, context.RetailerCode);

                        var totalPayment = result?.FirstOrDefault(f => f.PayslipId == payslipId)?.TotalPayment;
                        Logger.LogInformation("payslipId: " + payslip.Id + ",totalPayment: " + totalPayment);
                        if (totalPayment.HasValue)
                        {
                            payslip.UpdateTotalPayment(totalPayment.Value);
                            _db.Payslips.Attach(payslip);
                            _db.Entry(payslip).Property(ps => ps.TotalPayment).IsModified = true;
                            await _db.SaveChangesAsync();
                        }

                    }
                    else
                    {
                        throw new KvTimeSheetPayslipNullException($"Can't find payslip with id {payslipId}");
                    }
                }
            });
        }
    }
}
