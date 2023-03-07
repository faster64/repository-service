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

namespace KiotVietTimeSheet.Application.Commands.CreateCommissionDataTrial
{
    public class CreateCommissionDataTrialCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateCommissionDataTrialCommand, CommissionDto>
    {
        private readonly IMapper _mapper;
        private readonly ICommissionWriteOnlyRepository _commissionWriteOnlyRepository;
        public CreateCommissionDataTrialCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            ICommissionWriteOnlyRepository commissionWriteOnlyRepository
        ) : base(eventDispatcher)
        {
            _mapper = mapper;
            _commissionWriteOnlyRepository = commissionWriteOnlyRepository;
        }

        public async Task<CommissionDto> Handle(CreateCommissionDataTrialCommand request, CancellationToken cancellationToken)
        {
            var commissionDto = request.Commission;
            var commission = new Commission(commissionDto.Name, commissionDto.BranchIds, commissionDto.IsActive, commissionDto.IsAllBranch);

            _commissionWriteOnlyRepository.Add(commission);

            await _commissionWriteOnlyRepository.UnitOfWork.CommitAsync(false);

            return _mapper.Map<CommissionDto>(commission);
        }
    }
}
