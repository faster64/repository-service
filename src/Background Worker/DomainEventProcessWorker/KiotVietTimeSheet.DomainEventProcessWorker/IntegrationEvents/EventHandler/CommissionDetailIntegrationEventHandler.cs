using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents;
using KiotVietTimeSheet.Application.Runtime.Exception;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Factories;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.DomainEventProcessWorker.Exceptions;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Polly;
using ServiceStack;
using ServiceStack.Configuration;

namespace KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.EventHandler
{
    public class CommissionDetailIntegrationEventHandler :
        IIntegrationEventHandler<CreatedCommissionDetailByProductIntegrationEvent>,
        IIntegrationEventHandler<CreatedCommissionDetailByProductCategoryIntegrationEvent>,
        IIntegrationEventHandler<CreatedCommissionDetailByProductCategoryAsyncIntegrationEvent>,
        IIntegrationEventHandler<DeletedCommissionDetailIntegrationEvent>,
        IIntegrationEventHandler<UpdatedValueOfCommissionDetailIntegrationEvent>
    {
        private EfDbContext _db;
        private readonly IKiotVietApiClient _kiotVietApiClient;
        private readonly ICacheClient _cacheClient;
        private readonly IAppSettings _appSettings;
        private readonly Helper _helper = new Helper();
        private readonly CommissionDetailAuditProcess _commissionDetailAuditProcess;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private ILogger<CommissionDetailIntegrationEventHandler> _logger;
        private ILogger<CommissionDetailIntegrationEventHandler> Logger => _logger ?? (_logger = HostContext.Resolve<ILogger<CommissionDetailIntegrationEventHandler>>());
        private readonly int _pageNationCommissionDetailToSync = 10000;

        public CommissionDetailIntegrationEventHandler(
            EfDbContext db,
            IKiotVietApiClient kiotVietApiClient,
            IAppSettings appSettings,
            ICacheClient cacheClient,
            CommissionDetailAuditProcess commissionDetailAuditProcess,
            IKiotVietServiceClient kiotVietServiceClient)
        {
            _commissionDetailAuditProcess = commissionDetailAuditProcess;
            _kiotVietServiceClient = kiotVietServiceClient;
            _db = db;
            _kiotVietApiClient = kiotVietApiClient;
            _appSettings = appSettings;
            _cacheClient = cacheClient;
        }

        public async Task Handle(CreatedCommissionDetailByProductIntegrationEvent @event)
        {
            await CreateProductForCommissionSync(@event.ListCommissionDetails, @event.Context);
            await _commissionDetailAuditProcess.WriteCreateCommissionDetailByProductLogAsync(@event);
        }

        public async Task Handle(CreatedCommissionDetailByProductCategoryIntegrationEvent @event)
        {
            await _commissionDetailAuditProcess.WriteCreateCommissionDetailByProductCategoryLogAsync(@event);
        }

        public async Task Handle(CreatedCommissionDetailByProductCategoryAsyncIntegrationEvent @event)
        {
            await CreateCommissionDetailByCategorySync(@event.CommissionIds, @event.ProductCategory, @event.Context);
            await _commissionDetailAuditProcess.WriteCreateCommissionDetailByProductCategoryAsyncLogAsync(@event);
        }

        public async Task Handle(DeletedCommissionDetailIntegrationEvent @event)
        {
            await DeleteProductForCommissionSync(@event.ListCommissionDetails, @event.Context);
            await _commissionDetailAuditProcess.WriteDeleteCommissionDetailLogAsync(@event);
        }

        public async Task Handle(UpdatedValueOfCommissionDetailIntegrationEvent @event)
        {
            await _commissionDetailAuditProcess.WriteUpdateValueOfCommissionDetailLogAsync(@event);
        }

        private async Task CreateProductForCommissionSync(List<CommissionDetailDto> commissionDetails, IntegrationEventContext context)
        {
            var policy = Policy
                .Handle<KvTimeSheetPayslipNullException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (ex, time) =>
                    {
                        Logger.LogWarning($@"Retry create commission sync  after {time.TotalSeconds}s");
                    });

            await policy.ExecuteAsync(async () =>
            {
                if (commissionDetails.Any())
                    await _kiotVietServiceClient.CreateCommissionSync(commissionDetails, context);
            });
        }
        private async Task CreateCommissionDetailByCategorySync(List<long> commissionIds, ProductCategoryReqDto productCategory, IntegrationEventContext context)
        {
            var policy = Policy
                .Handle<KvTimeSheetPayslipNullException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (ex, time) =>
                    {
                        Logger.LogWarning($@"Retry create commission sync  after {time.TotalSeconds}s");
                    });

            await policy.ExecuteAsync(async () =>
            {
                if (!commissionIds.Any() && productCategory == null) return;

                using (_db = await _helper.GetDbContextByGroupId(context.GroupId,
                    context.RetailerCode, _cacheClient, _kiotVietApiClient, _appSettings))
                {
                    var keyObject = $@"{CacheKeys.GetEntityCacheKey(
                        context.RetailerCode,
                        nameof(KiotVietTimeSheet),
                        nameof(InsertCommissionDetailsStatus)
                    )}*";
                    var insertCommissionDetailsStatus = _cacheClient.GetOrDefault<InsertCommissionDetailsStatus>(keyObject);
                    try
                    {

                        if (insertCommissionDetailsStatus != null && insertCommissionDetailsStatus.Status == InsertCommissionDetailStatusEnums.InProgress)
                        {
                            var ex = new KvTimeSheetException(insertCommissionDetailsStatus.Message);
                            throw ex;
                        }

                        insertCommissionDetailsStatus = new InsertCommissionDetailsStatus
                        {
                            Status = InsertCommissionDetailStatusEnums.InProgress,
                            Message = Message.commission_InProgress
                        };

                        _cacheClient.Set(keyObject, insertCommissionDetailsStatus);

                        var listProductWithCategory = await _kiotVietServiceClient.GetListProductByCategoryId(new GetProductByCategoryIdReq
                        {
                            CategoryId = productCategory.Id,
                            RetailerId = context.TenantId
                        }, context);

                        var commissionDetails = await _db.CommissionDetails.Where(c => commissionIds.Contains(c.CommissionId)).ToListAsync();
                        var productIds = commissionDetails.Select(c => c.ProductId);
                        var listProduct = listProductWithCategory.Where(x => !productIds.Contains(x.Id)).ToList();

                        var newCommissionDetails = RenderCommissionDetails(context, listProduct, commissionIds);
                        await HasExistedProductGroup(newCommissionDetails, listProductWithCategory, commissionIds, insertCommissionDetailsStatus);
                        await SaveCommissionDetails(context, newCommissionDetails, insertCommissionDetailsStatus, keyObject);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        insertCommissionDetailsStatus = new InsertCommissionDetailsStatus
                        {
                            Status = InsertCommissionDetailStatusEnums.Error,
                            Message = ex.Message
                        };
                        _cacheClient.Set(keyObject, insertCommissionDetailsStatus);
                    }
                }
            });
        }

        private async Task DeleteProductForCommissionSync(List<CommissionDetailDto> commissionDetails, IntegrationEventContext context)
        {
            var policy = Policy
                .Handle<KvTimeSheetPayslipNullException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (ex, time) =>
                    {
                        Logger.LogWarning($@"Retry delete commission sync  after {time.TotalSeconds}s");
                    });

            await policy.ExecuteAsync(async () =>
            {
               
            });
        }

        private async Task<List<Paysheet>> WithCommissionTableDataChangeAsync(EfDbContext db, IntegrationEventContext context, List<long> commissionTableIds)
        {
            var payslipIds = (await db.PayslipDetails
                    .Where(x => x.TenantId == context.TenantId && x.RuleType == nameof(CommissionSalaryRuleV2))
                    .ToListAsync())
                .Where(x =>
                {
                    var rule = RuleFactory.GetRule(x.RuleType, x.RuleValue);
                    var isCommissionRule = rule?.GetType() == typeof(CommissionSalaryRuleV2);
                    if (rule == null || !isCommissionRule) return false;

                    var ruleParam = JsonConvert.DeserializeObject(x.RuleParam, typeof(CommissionSalaryRuleParamV2)) as CommissionSalaryRuleParamV2;
                    if (ruleParam == null || ruleParam.Type != CommissionSalaryTypes.WithTotalCommission) return false;

                    var isUsingCommissionTable = ruleParam.CommissionParams.Any(cp =>
                        cp.CommissionTable != null && commissionTableIds.Contains(cp.CommissionTable.Id));
                    return isUsingCommissionTable;
                })
                .Select(x => x.PayslipId)
                .Distinct();

            var paysheetIds = db.Payslips
                .Where(x => x.TenantId == context.TenantId)
                .Where(x => x.PayslipStatus != (byte)PayslipStatuses.PaidSalary &&
                            x.PayslipStatus != (byte)PayslipStatuses.Void)
                .Where(x => payslipIds.Contains(x.Id))
                .Select(x => x.PaysheetId);

            var paysheets = await db.Paysheet
                .Where(x => x.TenantId == context.TenantId)
                .Where(x => paysheetIds.Contains(x.Id))
                .ToListAsync();

            paysheets.ForEach(p => p.Version = p.Version + 1);

            return paysheets;
        }

        private static List<CommissionDetail> RenderCommissionDetails(IntegrationEventContext context, IReadOnlyCollection<ProductDto> listProduct, IEnumerable<long> commissionIds)
        {
            var newCommissionDetails = new List<CommissionDetail>();
            foreach (var commissionId in commissionIds)
            {
                newCommissionDetails.AddRange(
                    listProduct.Select(product =>
                        new CommissionDetail(context.TenantId, context.UserId, commissionId, product.Id, 0, null)));
            }
            return newCommissionDetails;
        }

        private async Task SaveCommissionDetails(IntegrationEventContext context, List<CommissionDetail> newCommissionDetails, InsertCommissionDetailsStatus insertCommissionDetailsStatus, string keyObject)
        {
            if (newCommissionDetails.Any() && newCommissionDetails.Count > 0)
            {
                var commissionTableIds = new List<long>();
                while (newCommissionDetails.Any())
                {
                    var paginationNewCommissionDetails = newCommissionDetails;
                    if (newCommissionDetails.Count > _pageNationCommissionDetailToSync)
                        paginationNewCommissionDetails =
                            newCommissionDetails.GetRange(0, _pageNationCommissionDetailToSync);

                    newCommissionDetails = newCommissionDetails.Except(paginationNewCommissionDetails).ToList();

                    commissionTableIds.AddRange(paginationNewCommissionDetails.Select(x => x.CommissionId).Distinct().ToList());
                    await _db.CommissionDetails.AddRangeAsync(paginationNewCommissionDetails);
                    await _kiotVietServiceClient.CreateCommissionSync(paginationNewCommissionDetails.ConvertTo<List<CommissionDetailDto>>(), context);
                    Thread.Sleep(1500);
                }

                if (commissionTableIds.Count == 0) return;

                var paysheets = await WithCommissionTableDataChangeAsync(_db, context, commissionTableIds);
                _db.Paysheet.AddRange(paysheets);

                await _db.SaveChangesAsync();
                insertCommissionDetailsStatus.Status = InsertCommissionDetailStatusEnums.Completed;
                insertCommissionDetailsStatus.Message = Message.commissionDetail_updateToCommission;
            }

            _cacheClient.Set(keyObject, insertCommissionDetailsStatus);
        }

        private async Task HasExistedProductGroup(IEnumerable<CommissionDetail> newCommissionDetails, IEnumerable<ProductDto> listProductWithCategory, ICollection<long> commissionIds, InsertCommissionDetailsStatus insertCommissionDetailsStatus)
        {
            if (!newCommissionDetails.Any() && listProductWithCategory.Any())
            {
                var commissions = await _db.Commission.Where(c => commissionIds.Contains(c.Id)).ToListAsync();
                var commissionNames = string.Join(", ", commissions.Select(x => x.Name).ToList());
                insertCommissionDetailsStatus.Status = InsertCommissionDetailStatusEnums.Error;
                insertCommissionDetailsStatus.Message = string.Format(Message.commission_ProductGroupExist, commissionNames);
            }
        }
    }

}
