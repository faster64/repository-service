using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.Validators.CommissionValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UpdateCommission
{
    public class UpdateCommissionCommandHandler : BaseCommandHandler,
       IRequestHandler<UpdateCommissionCommand, CommissionDto>
    {
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly ICommissionWriteOnlyRepository _commissionWriteOnlyRepository;
        private readonly ICommissionBranchWriteOnlyRepository _commissionBranchWriteOnlyRepository;
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IEventDispatcher _eventDispatcher;
        public UpdateCommissionCommandHandler(
           IEventDispatcher eventDispatcher,
           IAuthService authService,
           IMapper mapper,
           ICommissionWriteOnlyRepository commissionWriteOnlyRepository,
           ICommissionBranchWriteOnlyRepository commissionBranchWriteOnlyRepository,
           ICommissionReadOnlyRepository commissionReadOnlyRepository,
           IKiotVietServiceClient kiotVietServiceClient
       ) : base(eventDispatcher)
        {
            _mapper = mapper;
            _authService = authService;
            _commissionWriteOnlyRepository = commissionWriteOnlyRepository;
            _commissionBranchWriteOnlyRepository = commissionBranchWriteOnlyRepository;
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<CommissionDto> Handle(UpdateCommissionCommand request, CancellationToken cancellationToken)
        {
            var commissionDto = request.Commission;
            var existingCommission = await _commissionWriteOnlyRepository.FindBySpecificationAsync(new FindByEntityIdLongSpec<Commission>(commissionDto.Id), "CommissionBranches");
            var existingCommissionBranches = await _commissionBranchWriteOnlyRepository.GetBySpecificationAsync(new FindCommissionBranchByCommissionIdSpec(commissionDto.Id));

            if (existingCommission == null)
            {
                NotifyCommissionInDbIsNotExists();
                return null;
            }

            existingCommission.Update(commissionDto.Name, commissionDto.BranchIds, commissionDto.IsActive, commissionDto.IsAllBranch);
            var resultValidate = await (new CreateOrUpdateCommissionAsyncValidator(_commissionReadOnlyRepository, existingCommission, _kiotVietServiceClient, _authService)).ValidateAsync(existingCommission);

            if (!resultValidate.IsValid)
            {
                NotifyValidationErrors(typeof(Commission), resultValidate.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }

            _commissionWriteOnlyRepository.Update(existingCommission);
            if (existingCommissionBranches.Any())
                _commissionBranchWriteOnlyRepository.BatchDelete(existingCommissionBranches);

            var commissionBranches = commissionDto.BranchIds
                .Where(branchId => branchId > 0)
                .Select(branchId => new CommissionBranch(branchId, commissionDto.Id))
                .ToList();
            _commissionBranchWriteOnlyRepository.BatchAdd(commissionBranches);
            await _commissionWriteOnlyRepository.UnitOfWork.CommitAsync();

            var result = _mapper.Map<CommissionDto>(existingCommission);
            return result;
        }
        private void NotifyCommissionInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(nameof(Commission), Message.commission_NotExist));
        }
    }
}
