using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Commands.UpdatePayslip;
using KiotVietTimeSheet.Application.Queries.GetPayslipById;
using KiotVietTimeSheet.BackgroundTasks.Exceptions;
using KiotVietTimeSheet.BackgroundTasks.IntegrationEvents.Events;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Microsoft.Extensions.Logging;
using MediatR;
using KiotVietTimeSheet.Application.Auth;
using System.Linq;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;

namespace KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types
{
    public class PayslipPaymentProcess : BaseBackgroundProcess
    {
        private readonly ILogger<PayslipPaymentProcess> _logger;
        private readonly IMediator _mediator;

        public PayslipPaymentProcess(
            ILogger<PayslipPaymentProcess> logger,
            IKiotVietInternalService kiotVietInternalService,
            IAuthService authService,
            IMediator mediator) : base(kiotVietInternalService, authService)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task VoidedPayslipTotalPaymentAsync(VoidedPayslipPaymentIntegrationEvent @event)
        {
            await UpdatePayslipTotalPaymentAsync(@event.PayslipId, @event.Context);
        }

        public async Task CreatedPayslipPaymentAsync(CreatedPayslipPaymentIntegrationEvent @event)
        {
            await UpdatePayslipTotalPaymentAsync(@event.PayslipId, @event.Context);
        }

        private async Task UpdatePayslipTotalPaymentAsync(long payslipId, IntegrationEventContext context)
        {
            var payslip = await _mediator.Send(new GetPayslipByIdQuery(payslipId));

            if (payslip != null)
            {

                var result = await _kiotVietInternalService.GetTotalPaymentByPayslipId(
                    new GetTotalPaymentByPayslipIdsReq
                    {
                        RetailerId = payslip.TenantId,
                        PayslipIds = new List<long> { payslipId }
                    }, context.GroupId, context.RetailerCode);

                var totalPayment = result?.FirstOrDefault(f => f.PayslipId == payslipId)?.TotalPayment;
                _logger.LogInformation("payslipId: " + payslip.Id + ",totalPayment: " + totalPayment + ",payslipStatus:" + payslip.PayslipStatus);
                if (totalPayment.HasValue)
                {
                    await _mediator.Send(new UpdatePayslipCommand(payslipId, totalPayment));
                }
            }
            else
            {
                throw new KvTimeSheetPayslipNullException($"Can't find payslip with id {payslipId}");
            }
        }
    }
}
