using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications;
using System;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;

namespace KiotVietTimeSheet.Application.Commands.CreateCommissionDetail
{
    public class CreateCommissionDetailCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateCommissionDetailCommand, List<CommissionDetailDto>>
    {
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly ICommissionDetailWriteOnlyRepository _commissionDetailWriteOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IEventDispatcher _eventDispatcher;

        public CreateCommissionDetailCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IMapper mapper,
            ICommissionDetailWriteOnlyRepository commissionDetailWriteOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService
        )
            : base(eventDispatcher)
        {
            _authService = authService;
            _mapper = mapper;
            _commissionDetailWriteOnlyRepository = commissionDetailWriteOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<List<CommissionDetailDto>> Handle(CreateCommissionDetailCommand request, CancellationToken cancellationToken)
        {
            var commissionDetailDtoList = request.CommissionDetailDtoList;

            if (!IsValidCommissionDetails(commissionDetailDtoList))
            {
                return null;
            }

            var commissionDetails = 
                commissionDetailDtoList
                    .Select(commissionDetail => new CommissionDetail(commissionDetail.CommissionId, commissionDetail.ObjectId, commissionDetail.Value, commissionDetail.ValueRatio))
                    .ToList();

            await DeleteExisted(commissionDetails);

            _commissionDetailWriteOnlyRepository.BatchAdd(commissionDetails);
            var result = new List<CommissionDetailDto>();
            if (commissionDetails.Any() && commissionDetails.Count > 0)
            {
                // add audit trial, fire event
                var commission = commissionDetailDtoList.First();
                var product = new ProductCommissionDetailDto
                {
                    Id = commission.ObjectId,
                    Name = commission.ProductName
                };

                var context = _authService.Context;

                //sync CommissionDetail to FnB or Retailer
                result = _mapper.Map<List<CommissionDetailDto>>(commissionDetails);

                await _kiotVietServiceClient.ReCreateCommissionDetailSync(new GetProductByCategoryIdReq
                {
                    CommissionDetails = result
                }, _authService.Context);

                if (!request.IsNotAudit)
                {
                    await _timeSheetIntegrationEventService.AddEventAsync(
                        new CreatedCommissionDetailByProductIntegrationAuditEvent(commissionDetailDtoList, product));
                }
            }
            await _commissionDetailWriteOnlyRepository.UnitOfWork.CommitAsync();
            return result;
        }

        private bool IsValidCommissionDetails(List<CommissionDetailDto> commissionDetailDtoList)
        {
            if (commissionDetailDtoList == null || commissionDetailDtoList.Count == 0)
            {
                NotifyCommissionHaveNotSelected();
                return false;
            }

            if (commissionDetailDtoList.Any(cm => cm.CommissionId == 0 || cm.ObjectId == 0))
            {
                NotifyCommissionIdNotValid();
                return false;
            }

            return true;
        }

        private async Task DeleteExisted(ICollection<CommissionDetail> commissionDetails)
        {
            var existedDetails = await _commissionDetailWriteOnlyRepository.GetBySpecificationAsync(new FindCommissionDetailByExistedSpec(_authService.Context.TenantId, commissionDetails));
            if (existedDetails.Any())
            {
                var updateDetails = new List<CommissionDetail>();
                foreach (var commissionDetail in commissionDetails)
                {
                    var ls = existedDetails
                        .Where(x => x.TenantId == _authService.Context.TenantId &&
                                    x.CommissionId == commissionDetail.CommissionId &&
                                    x.ObjectId == commissionDetail.ObjectId)
                        .ToList();
                    if (ls.Any())
                    {
                        ls.ForEach(x=>x.Delete());
                        updateDetails.AddRange(ls);
                    }
                }
                if (updateDetails.Any())
                {
                    _commissionDetailWriteOnlyRepository.BatchUpdate(updateDetails);
                }
            }
        }

        private void NotifyCommissionHaveNotSelected()
        {
            _eventDispatcher.FireEvent(new DomainNotification(nameof(CommissionDetail), Message.commission_haveNotSelected));
        }

        private void NotifyCommissionIdNotValid()
        {
            _eventDispatcher.FireEvent(new DomainNotification(nameof(CommissionDetail), Message.commission_NotExist));
        }
    }
}
