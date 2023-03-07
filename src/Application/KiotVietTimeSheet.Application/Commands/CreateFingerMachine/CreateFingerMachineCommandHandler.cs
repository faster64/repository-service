using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.FingerPrintValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CreateFingerMachine
{
    public class CreateFingerMachineCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateFingerMachineCommand, FingerMachineDto>
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IFingerMachineWriteOnlyRepository _fingerMachineWriteOnlyRepository;
        private readonly IFingerMachineReadOnlyRepository _fingerMachineReadOnlyRepository;

        public CreateFingerMachineCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IMapper mapper,
            IFingerMachineWriteOnlyRepository fingerMachineWriteOnlyRepository,
            IFingerMachineReadOnlyRepository fingerMachineReadOnlyRepository
        )
            : base(eventDispatcher)
        {
            _authService = authService;
            _fingerMachineWriteOnlyRepository = fingerMachineWriteOnlyRepository;
            _mapper = mapper;
            _fingerMachineReadOnlyRepository = fingerMachineReadOnlyRepository;
        }

        public async Task<FingerMachineDto> Handle(CreateFingerMachineCommand request, CancellationToken cancellationToken)
        {
            var fingerMachineDto = request.FingerMachine;
            var fingerMachine = new FingerMachine(
                fingerMachineDto.BranchId ?? _authService.Context.BranchId,
                fingerMachineDto.MachineName,
                fingerMachineDto.Vendor,
                fingerMachineDto.MachineId,
                fingerMachineDto.Note,
                fingerMachineDto.Status,
                fingerMachineDto.IpAddress,
                fingerMachineDto.Commpass,
                fingerMachineDto.Port,
                fingerMachineDto.ConnectionType
            );

            var validateResult = await new CreateFingerMachineValidator(_fingerMachineReadOnlyRepository, fingerMachine).ValidateAsync(fingerMachine, cancellationToken);

            if (!validateResult.IsValid)
            {
                NotifyValidationErrors(typeof(FingerMachine), validateResult.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }

            _fingerMachineWriteOnlyRepository.Add(fingerMachine);


            await _fingerMachineWriteOnlyRepository.UnitOfWork.CommitAsync();
            return _mapper.Map<FingerMachineDto>(fingerMachine);
        }
    }
}
