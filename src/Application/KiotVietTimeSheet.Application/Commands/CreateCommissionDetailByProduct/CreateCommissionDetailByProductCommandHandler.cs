using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByProduct
{
    public class CreateCommissionDetailByProductCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateCommissionDetailByProductCommand, List<CommissionDetailDto>>
    {
        private readonly IMapper _mapper;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ICommissionDetailWriteOnlyRepository _commissionDetailWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IAuthService _authService;

        public CreateCommissionDetailByProductCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            ICommissionDetailWriteOnlyRepository commissionDetailWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IKiotVietServiceClient kiotVietServiceClient,
            IAuthService authService
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _mapper = mapper;
            _commissionDetailWriteOnlyRepository = commissionDetailWriteOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _kiotVietServiceClient = kiotVietServiceClient;
            _authService = authService;
        }

        public async Task<List<CommissionDetailDto>> Handle(CreateCommissionDetailByProductCommand request, CancellationToken cancellationToken)
        {
            var listCommissionIds = request.CommissionIds;
            var product = request.Product;
            if (listCommissionIds == null)
            {
                FireEvent(Message.commission_haveNotSelected);
                return null;
            }
            if (_authService?.Context == null)
            {
                FireEvent("Context is null");
                return null;
            }

            var newCommissionDetails = new List<CommissionDetail>();
            var needAddCommissionDetails = new List<CommissionDetail>();
            var commissionDetailSpecification = new FindCommissionDetailByCommissionIdsSpec(listCommissionIds).And(new FindCommissionDetailByTypeSpec((byte)CommissionDetailType.Product));
            var commissionDetails =
                await _commissionDetailWriteOnlyRepository.GetBySpecificationAsync(commissionDetailSpecification);

            foreach (var commissionId in listCommissionIds)
            {
                var isExistedCommissionDetail = commissionDetails.Exists(x => x.CommissionId == commissionId
                                                                                && x.ObjectId == product.Id);
                var newCommissionDetail = new CommissionDetail(commissionId, product.Id, _authService.Context.TenantId, 0, null);
                newCommissionDetails.Add(newCommissionDetail);
                if (!isExistedCommissionDetail)
                {
                    needAddCommissionDetails.Add(newCommissionDetail);
                }
            }

            var result = _mapper.Map<List<CommissionDetailDto>>(newCommissionDetails);

            if (needAddCommissionDetails.Any())
            {
                _commissionDetailWriteOnlyRepository.BatchAdd(needAddCommissionDetails);
            }

            if (result.Any())
            {
                await _kiotVietServiceClient.ReCreateCommissionDetailSync(new GetProductByCategoryIdReq
                {
                    CommissionDetails = result
                }, _authService.Context);
            }

            // add audit trial, fire event
            if (newCommissionDetails.Any() && newCommissionDetails.Count > 0)
            {
                await _timeSheetIntegrationEventService.AddEventAsync(new CreatedCommissionDetailByProductIntegrationAuditEvent(result, product));
            }
            await _commissionDetailWriteOnlyRepository.UnitOfWork.CommitAsync();
            return result;
        }

        #region Private

        private void FireEvent(string msg)
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(CommissionDetail).Name, msg));
        }

        #endregion Private
    }
}