using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.EventBus;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Commands.CreateMultipleCommissionDetail
{
    public class CreateMultipleCommissionDetailCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateMultipleCommissionDetailCommand, List<CommissionDetailDto>>
    {
        private readonly IMapper _mapper;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IAuthService _authService;
        private readonly ICommissionDetailWriteOnlyRepository _commissionDetailWriteOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;

        public CreateMultipleCommissionDetailCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IMapper mapper,
            ICommissionDetailWriteOnlyRepository commissionDetailWriteOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _authService = authService;
            _mapper = mapper;
            _commissionDetailWriteOnlyRepository = commissionDetailWriteOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public decimal? GetValue(decimal? value, decimal? valueRatio)
        {
            var reValue = value;
            if (value == null && valueRatio == null) reValue = 0;
            return reValue;
        }

        public decimal? GetValueRatio(decimal? value, decimal? valueRatio)
        {
            var reValueRatio = valueRatio;
            if (value != null && valueRatio != null) reValueRatio = null;
            return reValueRatio;
        }

        public bool CheckTotalComission(ProductCommissionDetailDto product, List<long> totalCommissionIds)
        {
            if (product.CommissionId > 0 && totalCommissionIds != null && totalCommissionIds.Any()) return true;
            NotifyCommissionHaveNotSelected();
            return false;

        }
        public async Task<List<CommissionDetailDto>> Handle(CreateMultipleCommissionDetailCommand request, CancellationToken cancellationToken)
        {
            var context = _authService.Context;
            var product = request.Product;
            var totalCommissionIds = request.TotalCommissionIds;
            var categoryCommissionDetail = request.Category;
            var value = request.Value;
            var valueRatio = request.ValueRatio;
            var isUpdateForAllCommission = request.IsUpdateForAllCommission;
            var categoryId = request.CategoryId;
            var productCodeKeyword = request.ProductCodeKeyword;
            var productNameKeyword = request.ProductNameKeyword;
            var newCommissionDetails = new List<CommissionDetail>();

            var eventCommissionDetails = new List<CommissionDetail>();
            CommissionDetail existCommissionDetail = await GetExistCommissionDetail(product, categoryCommissionDetail);
            var listIntegrationEvents = new List<IntegrationEvent>();
            if (existCommissionDetail != null)
            {
                NotifyCommissionDetailInDbAlreadyAddedBefore();
                return null;
            }

            if (categoryCommissionDetail == null && !CheckTotalComission(product, totalCommissionIds)) return null;

            value = GetValue(value, valueRatio);
            valueRatio = GetValueRatio(value, valueRatio);

            if (value != null && valueRatio != null) valueRatio = null;

            if (isUpdateForAllCommission)
            {
                await UpdateValueForAllCommissions(product, totalCommissionIds, value, valueRatio, categoryId, productCodeKeyword,
                                                        productNameKeyword, eventCommissionDetails, listIntegrationEvents);
            }

            CommissionDetail newCommissionDetailItem = MakeNewCommissionDetail(product, categoryCommissionDetail, value, valueRatio);

            _commissionDetailWriteOnlyRepository.Add(newCommissionDetailItem);
            newCommissionDetails.Add(newCommissionDetailItem);
            eventCommissionDetails.Add(newCommissionDetailItem);

            await _kiotVietServiceClient.CreateCommissionDetailSync(new GetProductByCategoryIdReq
            {
                CommissionDetails = _mapper.Map<List<CommissionDetailDto>>(newCommissionDetails),
                RetailerId = context.TenantId,
                GroupId = context.User.GroupId,
                UserId = context.User.Id
            });

            listIntegrationEvents.Add(new CreatedCommissionDetailByProductIntegrationEvent(
                _mapper.Map<List<CommissionDetailDto>>(new List<CommissionDetail>
                {
                    newCommissionDetailItem
                }), product));

            if (listIntegrationEvents.Any())
            {
                await _timeSheetIntegrationEventService.AddMultiEventAsync(listIntegrationEvents);
            }

            await _commissionDetailWriteOnlyRepository.UnitOfWork.CommitAsync();

            var result = _mapper.Map<List<CommissionDetailDto>>(eventCommissionDetails);

            return result;
        }

        private async Task UpdateValueForAllCommissions(ProductCommissionDetailDto product, List<long> totalCommissionIds, decimal? value,
            decimal? valueRatio, int categoryId, string productCodeKeyword, string productNameKeyword, List<CommissionDetail> eventCommissionDetails,
            List<IntegrationEvent> listIntegrationEvents)
        {
            var listProductCommissionDetails = await _kiotVietServiceClient.GetTimeSheetProductCommissionReq(new
                                GetTimeSheetProductCommissionReq
            {
                CategoryId = categoryId,
                CommissionIds = totalCommissionIds,
                ProductCodeKeyword = productCodeKeyword,
                ProductNameKeyword = productNameKeyword
            });

            if (listProductCommissionDetails.Data != null && listProductCommissionDetails.Data.Any())
            {
                var listProductIds = listProductCommissionDetails.Data.Select(x => x.Id).ToList();

                var commissionDetailNeedUpdates =
                    await _commissionDetailWriteOnlyRepository.GetBySpecificationAsync(
                        new FindCommissionDetailByCommissionIdAndProductIdsSpec(product.CommissionId, listProductIds));

                foreach (var commissionDetail in commissionDetailNeedUpdates)
                {
                    commissionDetail.UpdateValue(value);
                    commissionDetail.UpdateValueRatio(valueRatio);
                    eventCommissionDetails.Add(commissionDetail);
                }
                _commissionDetailWriteOnlyRepository.BatchUpdate(commissionDetailNeedUpdates);

                if (commissionDetailNeedUpdates.Any())
                {
                    listIntegrationEvents.Add(new UpdatedValueOfCommissionDetailIntegrationEvent(_mapper.Map<List<CommissionDetailDto>>(commissionDetailNeedUpdates), new List<ProductCommissionDetailDto>
                        {
                            product
                        }));
                }
                _commissionDetailWriteOnlyRepository.BatchUpdate(commissionDetailNeedUpdates);
            }
        }

        private static CommissionDetail MakeNewCommissionDetail(ProductCommissionDetailDto product, CategoryCommissionDetailDto categoryCommissionDetail, decimal? value, decimal? valueRatio)
        {
            return categoryCommissionDetail == null ? new CommissionDetail(product.CommissionId, product.Id, value, valueRatio) :
                                new CommissionDetail(categoryCommissionDetail.CommissionId, categoryCommissionDetail.Id, value, valueRatio, (byte)CommissionDetailType.Category);
        }

        private async Task<CommissionDetail> GetExistCommissionDetail(ProductCommissionDetailDto product, CategoryCommissionDetailDto categoryCommissionDetail)
        {
            return categoryCommissionDetail == null ? await
                            _commissionDetailWriteOnlyRepository.FindBySpecificationAsync(
                                new FindCommissionDetailByProducIdSpec(product.Id).And(new FindCommissionDetailByCommissionIdSpec(product.CommissionId))) :
                                await
                            _commissionDetailWriteOnlyRepository.FindBySpecificationAsync(
                                new FindCommissionDetailByCategoryIdSpec(categoryCommissionDetail.Id).And(new FindCommissionDetailByCommissionIdSpec(product.CommissionId)));
        }

        private void NotifyCommissionHaveNotSelected()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(CommissionDetail).Name, Message.commission_haveNotSelected));
        }
        private void NotifyCommissionDetailInDbAlreadyAddedBefore()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(CommissionDetail).Name, Message.commission_ProductExistInCommissionBefore));
        }
    }
}

