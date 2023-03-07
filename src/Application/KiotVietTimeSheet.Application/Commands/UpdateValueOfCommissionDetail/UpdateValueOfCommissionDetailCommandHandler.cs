using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
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
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UpdateValueOfCommissionDetail
{
    public class UpdateValueOfCommissionDetailCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateValueOfCommissionDetailCommand, List<CommissionDetailDto>>
    {
        private readonly IMapper _mapper;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ICommissionDetailWriteOnlyRepository _commissionDetailWriteOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;

        public UpdateValueOfCommissionDetailCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            ICommissionDetailWriteOnlyRepository commissionDetailWriteOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _mapper = mapper;
            _commissionDetailWriteOnlyRepository = commissionDetailWriteOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task<List<CommissionDetailDto>> Handle(UpdateValueOfCommissionDetailCommand request, CancellationToken cancellationToken)
        {
            var product = request.Product;
            var totalCommissionIds = request.TotalCommissionIds;
            var value = request.Value;
            var valueRatio = request.ValueRatio;
            var isUpdateForAllCommission = request.IsUpdateForAllCommission;
            var categoryId = request.CategoryId;
            var productCodeKeyword = request.ProductCodeKeyword;
            var productNameKeyword = request.ProductNameKeyword;

            var commissionDetailCategory = request.Category;

            var eventCommissionDetails = new List<CommissionDetail>();

            if (value == null && valueRatio == null) value = 0;
            if (value != null && valueRatio != null) valueRatio = null;

            //Cập nhập cho tất cả hàng hóa trong bảng hoa hồng
            if (isUpdateForAllCommission)
            {
                var commissionId = product != null && product.CommissionId > 0 ? product.CommissionId : commissionDetailCategory.CommissionId;
                eventCommissionDetails = await UpdateForAllCommission(categoryId, totalCommissionIds, productCodeKeyword,
                    productNameKeyword, value, valueRatio, commissionId);
            }
            else
            {
                var commissionDetailSpec = new FindCommissionDetailByCommissionIdSpec(product.CommissionId).And(
                        new FindCommissionDetailByProducIdSpec(product.Id));

                if (commissionDetailCategory != null)
                {
                    var categoryCommissionDetailSpec = new FindCommissionDetailByCommissionIdSpec(commissionDetailCategory.CommissionId).And(
                        new FindCommissionDetailByCategoryIdSpec(commissionDetailCategory.Id)).And(new FindCommissionDetailByTypeSpec((byte)CommissionDetailType.Category));
                    commissionDetailSpec = commissionDetailSpec.Or(categoryCommissionDetailSpec);
                }

                var commissionDetail = await _commissionDetailWriteOnlyRepository.FindBySpecificationAsync(commissionDetailSpec);
                if (commissionDetail == null)
                {
                    NotifyCommissionDetailInDbIsNotExists();
                    return null;
                }

                commissionDetail.UpdateValue(value);
                commissionDetail.UpdateValueRatio(valueRatio);
                _commissionDetailWriteOnlyRepository.Update(commissionDetail);
                eventCommissionDetails.Add(commissionDetail);
            }

            // Audit trail
            var result = _mapper.Map<List<CommissionDetailDto>>(eventCommissionDetails);
            await _timeSheetIntegrationEventService.AddEventAsync(new UpdatedValueOfCommissionDetailIntegrationEvent(result, new List<ProductCommissionDetailDto>
            {
                product
            }));
            await _commissionDetailWriteOnlyRepository.UnitOfWork.CommitAsync();

            return result;
        }
        private void NotifyCommissionDetailInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(CommissionDetail).Name, Message.commission_NotExist));
        }

        private async Task<List<CommissionDetail>> UpdateForAllCommission(int categoryId, List<long> totalCommissionIds, string productCodeKeyword, string productNameKeyword,
            decimal? value, decimal? valueRatio, long commissionId)
        {
            var eventAllCommissionDetails = new List<CommissionDetail>();
            var listProductCommissionDetail = await _kiotVietServiceClient.GetTimeSheetProductCommissionReq(new
                GetTimeSheetProductCommissionReq
            {
                CategoryId = categoryId,
                CommissionIds = totalCommissionIds,
                ProductCodeKeyword = productCodeKeyword,
                ProductNameKeyword = productNameKeyword
            });
            if (listProductCommissionDetail.Data == null || !listProductCommissionDetail.Data.Any())
                return new List<CommissionDetail>();

            var commissionDetailByFilter = listProductCommissionDetail.Data.Select(x => new CommissionDetailDto { ObjectId = x.Id, Type = x.Type }).ToList();

            var allCommissionDetailByCommisionIds =
                await _commissionDetailWriteOnlyRepository.GetBySpecificationAsync(
                    new FindCommissionDetailByCommissionIdSpec(commissionId));

            var commissionDetailNeedUpdates = allCommissionDetailByCommisionIds
                                            .Where(x => commissionDetailByFilter.Any(c => c.ObjectId == x.ObjectId && c.Type == x.Type))
                                            .ToList();

            foreach (var commissionDetail in commissionDetailNeedUpdates)
            {
                if (commissionDetail.Value != value || commissionDetail.ValueRatio != valueRatio)
                {
                    commissionDetail.UpdateValue(value);
                    commissionDetail.UpdateValueRatio(valueRatio);
                    eventAllCommissionDetails.Add(commissionDetail);
                }
            }
            _commissionDetailWriteOnlyRepository.BatchUpdate(commissionDetailNeedUpdates);

            return eventAllCommissionDetails;
        }
    }
}

