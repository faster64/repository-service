using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Commands.CreateCommissionDetailCategory
{
    public class CreateCommissionDetailCategoryCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateCommissionDetailCategoryCommand, List<CommissionDetailDto>>
    {
        private readonly IMapper _mapper;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IAuthService _authService;
        private readonly ICommissionDetailWriteOnlyRepository _commissionDetailWriteOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly ICommissionWriteOnlyRepository _commissionWriteOnlyRepository;

        public CreateCommissionDetailCategoryCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IMapper mapper,
            ICommissionWriteOnlyRepository commissionWriteOnlyRepository,
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
            _commissionWriteOnlyRepository = commissionWriteOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task<List<CommissionDetailDto>> Handle(CreateCommissionDetailCategoryCommand request, CancellationToken cancellationToken)
        {
            var commissionIdList = request.CommissionIds;
            var categoryIds = request.CategoryIds;
            if (!commissionIdList.Any())
            {
                NotifyCommissionHaveNotSelected();
            }

            var context = _authService.Context;
            var newCommissionDetails = new List<CommissionDetail>();
            var needAddCommissionDetails = new List<CommissionDetail>();

            var commissionDetailSpecification = new FindCommissionDetailByCommissionIdsSpec(commissionIdList).And(new FindCommissionDetailByTypeSpec((byte)CommissionDetailType.Category));
            var commissionDetails =
                await _commissionDetailWriteOnlyRepository.GetBySpecificationAsync(commissionDetailSpecification);

            foreach (var commissionId in commissionIdList)
            {
                foreach (var categoryId in categoryIds)
                {

                    var isExistedCommissionDetail = commissionDetails.Exists(x => x.CommissionId == commissionId
                                                                                && x.ObjectId == categoryId);
                    var newCommissionDetail = new CommissionDetail(commissionId, categoryId, _authService.Context.TenantId, 0, null, (byte)CommissionDetailType.Category);
                    newCommissionDetails.Add(newCommissionDetail);
                    if (!isExistedCommissionDetail)
                    {
                        needAddCommissionDetails.Add(newCommissionDetail);
                    }
                }
            }

            var result = _mapper.Map<List<CommissionDetailDto>>(newCommissionDetails);

            if (needAddCommissionDetails.Any())
            {
                _commissionDetailWriteOnlyRepository.BatchAdd(needAddCommissionDetails);
            }

            await _kiotVietServiceClient.ReCreateCommissionDetailSync(new GetProductByCategoryIdReq
            {
                CommissionDetails = result
            }, _authService.Context);

            await _timeSheetIntegrationEventService.AddEventAsync(
                new CreatedCommissionDetailCategoryAsyncIntegrationEvent(commissionIdList,
                    categoryIds));

            await _commissionDetailWriteOnlyRepository.UnitOfWork.CommitAsync();

            return _mapper.Map<List<CommissionDetailDto>>(result);
        }
        private void NotifyCommissionHaveNotSelected()
        {
            _eventDispatcher.FireEvent(new DomainNotification(nameof(CommissionDetail), Message.commission_haveNotSelected));
        }

        private void NotifyProductGroupExists(IEnumerable<string> listCommission)
        {
            _eventDispatcher.FireEvent(new DomainNotification(nameof(CommissionDetail), string.Format(Message.commission_ProductGroupExist, string.Join(", ", listCommission))));
        }
    }
}
