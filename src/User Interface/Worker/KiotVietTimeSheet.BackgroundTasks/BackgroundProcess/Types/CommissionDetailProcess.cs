using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByCategory;
using KiotVietTimeSheet.Application.Commands.UpdatePaysheets;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents;
using KiotVietTimeSheet.Application.Queries.GetAllCommissionDetailsByCommissionIds;
using KiotVietTimeSheet.Application.Queries.GetCommissionByIds;
using KiotVietTimeSheet.Application.Queries.GetPaysheetByIds;
using KiotVietTimeSheet.Application.Queries.GetPayslipById;
using KiotVietTimeSheet.Application.Queries.GetPayslipDetailByTenantId;
using KiotVietTimeSheet.Application.Runtime.Exception;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Factories;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using KiotVietTimeSheet.Resources;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types
{
    public class CommissionDetailProcess : BaseBackgroundProcess
    {
        private readonly ICacheClient _cacheClient;
        private readonly IMediator _mediator;
        private readonly ILogger<CommissionDetailProcess> _logger;
        private readonly IMapper _mapper;

        private const int PageNationCommissionDetailToSync = 10000;

        public CommissionDetailProcess(
            IKiotVietInternalService kiotVietInternalService,
            IAuthService authService,
            ICacheClient cacheClient,
            IMediator mediator,
            IMapper mapper,
            ILogger<CommissionDetailProcess> logger
        ) : base(kiotVietInternalService,
            authService)
        {
            _cacheClient = cacheClient;
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task CreateCommissionDetailByProductCategoryAsync(CreatedCommissionDetailByProductCategoryAsyncIntegrationEvent @event)
        {
            var keyObject = $@"{CacheKeys.GetEntityCacheKey(
                        _authService.Context.TenantCode,
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

                var listProductWithCategory = await _kiotVietInternalService.GetListProductByCategoryId(new GetProductByCategoryIdReq
                {
                    CategoryId = @event.ProductCategory.Id,
                    RetailerId = _authService.Context.TenantId
                });
                var commissionIds = @event.CommissionIds;
                var commissionDetails = await _mediator.Send(new GetAllCommissionDetailsByCommissionIdsQuery(commissionIds));
                var productIds = commissionDetails.Select(c => c.ObjectId);
                var listProduct = listProductWithCategory.Where(x => !productIds.Contains(x.Id)).ToList();

                var newCommissionDetails = RenderCommissionDetails(_authService.Context.TenantId, _authService.Context.User.Id, listProduct, commissionIds);
                await HasExistedProductGroup(newCommissionDetails, listProductWithCategory, commissionIds, insertCommissionDetailsStatus);
                await SaveCommissionDetails(newCommissionDetails, insertCommissionDetailsStatus, keyObject);
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

        private static List<CommissionDetail> RenderCommissionDetails(int tenantId, long userId, IReadOnlyCollection<ProductDto> listProduct, IEnumerable<long> commissionIds)
        {
            var newCommissionDetails = new List<CommissionDetail>();
            foreach (var commissionId in commissionIds)
            {
                newCommissionDetails.AddRange(
                    listProduct.Select(product =>
                        new CommissionDetail(tenantId, userId, commissionId, product.Id, 0, null)));
            }
            return newCommissionDetails;
        }

        private async Task HasExistedProductGroup(IEnumerable<CommissionDetail> newCommissionDetails, IEnumerable<ProductDto> listProductWithCategory, ICollection<long> commissionIds, InsertCommissionDetailsStatus insertCommissionDetailsStatus)
        {
            if (!newCommissionDetails.Any() && listProductWithCategory.Any())
            {
                var commissions = await _mediator.Send(new GetCommissionByIdsQuery(commissionIds.ToList()));
                var commissionNames = string.Join(", ", commissions.Select(x => x.Name).ToList());
                insertCommissionDetailsStatus.Status = InsertCommissionDetailStatusEnums.Error;
                insertCommissionDetailsStatus.Message = string.Format(Message.commission_ProductGroupExist, commissionNames);
            }
        }

        private async Task SaveCommissionDetails(List<CommissionDetail> newCommissionDetails, InsertCommissionDetailsStatus insertCommissionDetailsStatus, string keyObject)
        {
            if (newCommissionDetails.Any() && newCommissionDetails.Count > 0)
            {
                var commissionTableIds = new List<long>();
                while (newCommissionDetails.Any())
                {
                    var paginationNewCommissionDetails = newCommissionDetails;
                    if (newCommissionDetails.Count > PageNationCommissionDetailToSync)
                        paginationNewCommissionDetails =
                            newCommissionDetails.GetRange(0, PageNationCommissionDetailToSync);

                    newCommissionDetails = newCommissionDetails.Except(paginationNewCommissionDetails).ToList();

                    commissionTableIds.AddRange(paginationNewCommissionDetails.Select(x => x.CommissionId).Distinct().ToList());
                    var commissionDetailDtoList = _mapper.Map<List<CommissionDetailDto>>(paginationNewCommissionDetails);
                    await _mediator.Send(new CreateCommissionDetailByCategoryCommand(paginationNewCommissionDetails));

                    var commissionDetailReq = new GetProductByCategoryIdReq()
                    {
                        BranchId = _authService.Context.BranchId,
                        RetailerId = _authService.Context.TenantId,
                        UserId = _authService.Context.User.Id,
                        CommissionDetails = commissionDetailDtoList
                    };

                    await _kiotVietInternalService.CreateCommissionSync(commissionDetailReq);
                    Thread.Sleep(1500);
                }

                if (commissionTableIds.Count == 0) return;

                var paysheets = await WithCommissionTableDataChangeAsync(commissionTableIds);
                await _mediator.Send(new UpdatePaysheetsCommand(paysheets));
                insertCommissionDetailsStatus.Status = InsertCommissionDetailStatusEnums.Completed;
                insertCommissionDetailsStatus.Message = Message.commissionDetail_updateToCommission;
            }

            _cacheClient.Set(keyObject, insertCommissionDetailsStatus);
        }

        private async Task<List<Paysheet>> WithCommissionTableDataChangeAsync(List<long> commissionTableIds)
        {
            var payslipIds = (await _mediator.Send(new GetPayslipDetailByTenantIdQuery(_authService.Context.TenantId,
                    nameof(CommissionSalaryRuleV2))))
                .Where(x =>
                {
                    var rule = RuleFactory.GetRule(x.RuleType, x.RuleValue);
                    var isCommissionRule = rule?.GetType() == typeof(CommissionSalaryRuleV2);
                    if (rule == null || !isCommissionRule) return false;

                    if (!(JsonConvert.DeserializeObject(x.RuleParam, typeof(CommissionSalaryRuleParamV2)) is CommissionSalaryRuleParamV2 ruleParam) || ruleParam.Type != CommissionSalaryTypes.WithTotalCommission) return false;

                    var isUsingCommissionTable = ruleParam.CommissionParams.Any(cp =>
                        cp.CommissionTable != null && commissionTableIds.Contains(cp.CommissionTable.Id));
                    return isUsingCommissionTable;
                })
                .Select(x => x.PayslipId)
                .Distinct().ToList();

            var paysheetIds = await _mediator.Send(new GetPayslipByIdsQuery(payslipIds));
            var paysheets = await _mediator.Send(new GetPaysheetByIdsQuery(paysheetIds));
            paysheets.ForEach(p => p.Version += 1);

            return paysheets;
        }
    }
}
