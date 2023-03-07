using System.Linq;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.Validators.CommissionValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications;

namespace KiotVietTimeSheet.Application.Commands.DeleteCommissionDataTrial
{
    public class DeleteCommissionDataTrialCommandHandler : BaseCommandHandler,
        IRequestHandler<DeleteCommissionDataTrialCommand, CommissionDto>
    {
        private readonly IMapper _mapper;
        private readonly ICommissionWriteOnlyRepository _commissionWriteOnlyRepository;
        private readonly ICommissionDetailWriteOnlyRepository _commissionDetailWriteOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IEventDispatcher _eventDispatcher;
        public DeleteCommissionDataTrialCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            ICommissionWriteOnlyRepository commissionWriteOnlyRepository,
            ICommissionDetailWriteOnlyRepository commissionDetailWriteOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient
        ) : base(eventDispatcher)
        {
            _mapper = mapper;
            _commissionWriteOnlyRepository = commissionWriteOnlyRepository;
            _commissionDetailWriteOnlyRepository = commissionDetailWriteOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<CommissionDto> Handle(DeleteCommissionDataTrialCommand request, CancellationToken cancellationToken)
        {
            long id = request.Id;
            var existingCommission = await _commissionWriteOnlyRepository.FindBySpecificationAsync(new FindByEntityIdLongSpec<Commission>(id), "CommissionBranches");

            if (existingCommission == null)
            {
                NotifyCommissionInDbIsNotExists();
                return null;
            }

            existingCommission.Delete();

            _commissionWriteOnlyRepository.Delete(existingCommission);
            var productByCategoryIdReq = new GetProductByCategoryIdReq
            {
                UserId = request.UserIdAdmin,
                RetailerId = request.TenantId,
                BranchId = request.BranchId,
                GroupId = request.GroupId,
                RetailerCode = request.TenantCode
            };
            // detete data in CommissionDetail table
            await DeleteCommissionDetail(id);
            // detete data in TimeSheetCommissionSync table in MHQL
            await _kiotVietServiceClient.DeleteTimeSheetCommissionSync(productByCategoryIdReq, id);

            await _commissionWriteOnlyRepository.UnitOfWork.CommitAsync(false);

            var result = _mapper.Map<CommissionDto>(existingCommission);
            return result;
        }
        private async Task DeleteCommissionDetail(long commissionId)
        {
            var commissionDetailSpecifications = new FindCommissionDetailByCommissionIdSpec(commissionId);
            var existingCommissionDetailList = await _commissionDetailWriteOnlyRepository.GetBySpecificationAsync(commissionDetailSpecifications);
            foreach (var existingCommissionDetail in existingCommissionDetailList)
            {
                existingCommissionDetail.Delete();
            }

            _commissionDetailWriteOnlyRepository.BatchUpdate(existingCommissionDetailList);
            await _commissionDetailWriteOnlyRepository.UnitOfWork.CommitAsync(false);
        }
        private void NotifyCommissionInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(Commission).Name, Message.commission_NotExist));
        }
    }
}
