using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
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
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums;

namespace KiotVietTimeSheet.Application.Commands.DeleteCommissionDetail
{
    public class DeleteCommissionDetailCommandHandler : BaseCommandHandler,
        IRequestHandler<DeleteCommissionDetailCommand, List<CommissionDetailDto>>
    {
        private readonly IMapper _mapper;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ICommissionDetailWriteOnlyRepository _commissionDetailWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IAuthService _authService;

        public DeleteCommissionDetailCommandHandler(
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

        public async Task<List<CommissionDetailDto>> Handle(DeleteCommissionDetailCommand request, CancellationToken cancellationToken)
        {
            var commissionIds = request.CommissionIds;
            var products = request.Products;
            var categoryIds = request.CategoryIds == null ? new List<long>() : request.CategoryIds;


            var productCommissionDetailSpecifications = new FindCommissionDetailByCommissionIdsSpec(commissionIds).And(new FindCommissionDetailByProductIdsSpec(products.Select(x => x.Id).ToList()));
            var categoryCommissionDetailSpecifications = new FindCommissionDetailByCommissionIdsSpec(commissionIds)
                .And(new FindCommissionDetailByCategoryIdsSpec(categoryIds))
                .And(new FindCommissionDetailByTypeSpec((byte)CommissionDetailType.Category));
            
            var existingCommissionDetailList = await _commissionDetailWriteOnlyRepository.GetBySpecificationAsync(productCommissionDetailSpecifications.Or(categoryCommissionDetailSpecifications));
            
            if (existingCommissionDetailList == null)
            {
                NotifyCommissionDetailInDbIsNotExists();
                return null;
            }

            foreach (var existingCommissionDetail in existingCommissionDetailList)
            {
                existingCommissionDetail.Delete();
            }

            _commissionDetailWriteOnlyRepository.BatchUpdate(existingCommissionDetailList);

            // Audit trail

            var result = _mapper.Map<List<CommissionDetailDto>>(existingCommissionDetailList);

            var productByCategoryIdReq = new GetProductByCategoryIdReq
            {
                UserId = _authService.Context.User.Id,
                RetailerId = _authService.Context.TenantId,
                BranchId = _authService.Context.BranchId,
                GroupId = _authService.Context.User.GroupId,
                RetailerCode = _authService.Context.TenantCode,
                CommissionDetails = result
            };

            await _kiotVietServiceClient.DeleteCommissionSync(productByCategoryIdReq);
            await _timeSheetIntegrationEventService.AddEventAsync(new DeletedCommissionDetailIntegrationAuditEvent(result, products));
            await _commissionDetailWriteOnlyRepository.UnitOfWork.CommitAsync();


            return result;
        }
        private void NotifyCommissionDetailInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(nameof(CommissionDetail), Message.commission_NotExist));
        }
    }
}


