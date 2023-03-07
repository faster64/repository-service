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
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CreateCommission
{
    public class CreateCommissionCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateCommissionCommand, CommissionDto>
    {
        private readonly IMapper _mapper;
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly ICommissionWriteOnlyRepository _commissionWriteOnlyRepository;
        private readonly IAuthService _authService;
        public CreateCommissionCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            IKiotVietServiceClient kiotVietServiceClient,
            ICommissionWriteOnlyRepository commissionWriteOnlyRepository,
            IAuthService authService
        ) : base(eventDispatcher)
        {
            _mapper = mapper;
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
            _commissionWriteOnlyRepository = commissionWriteOnlyRepository;
            _authService = authService;
        }

        public async Task<CommissionDto> Handle(CreateCommissionCommand request, CancellationToken cancellationToken)
        {
            var commissionDto = request.Commission;
            var commission = new Commission(commissionDto.Name, commissionDto.BranchIds, commissionDto.IsActive, commissionDto.IsAllBranch);

            var validateResult = await (new CreateOrUpdateCommissionAsyncValidator(_commissionReadOnlyRepository, commission, _kiotVietServiceClient, _authService)).ValidateAsync(commission);

            if (!validateResult.IsValid)
            {
                NotifyValidationErrors(typeof(Commission), validateResult.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }

            _commissionWriteOnlyRepository.Add(commission);

            await _commissionWriteOnlyRepository.UnitOfWork.CommitAsync();

            return _mapper.Map<CommissionDto>(commission);
        }
    }
}
