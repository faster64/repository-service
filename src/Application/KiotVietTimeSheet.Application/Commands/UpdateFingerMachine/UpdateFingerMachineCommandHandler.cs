using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.FingerPrintValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UpdateFingerMachine
{
    public class UpdateFingerMachineCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateFingerMachineCommand, FingerMachineDto>
    {
        private readonly IMapper _mapper;
        private readonly IFingerMachineWriteOnlyRepository _fingerMachineWriteOnlyRepository;
        private readonly IFingerMachineReadOnlyRepository _fingerMachineReadOnlyRepository;

        public UpdateFingerMachineCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IFingerMachineWriteOnlyRepository fingerMachineWriteOnlyRepository,
            IFingerMachineReadOnlyRepository fingerMachineReadOnlyRepository
        )
            : base(eventDispatcher)
        {
            _fingerMachineWriteOnlyRepository = fingerMachineWriteOnlyRepository;
            _mapper = mapper;
            _fingerMachineReadOnlyRepository = fingerMachineReadOnlyRepository;
        }

        public async Task<FingerMachineDto> Handle(UpdateFingerMachineCommand request, CancellationToken cancellationToken)
        {
            var fingerMachineDto = request.FingerMachine;
            var existFingerMachine = await _fingerMachineWriteOnlyRepository.FindByIdAsync(fingerMachineDto.Id);

            existFingerMachine.Update(
                fingerMachineDto.MachineName,
                fingerMachineDto.MachineId,
                fingerMachineDto.IpAddress,
                fingerMachineDto.Port,
                fingerMachineDto.Commpass,
                fingerMachineDto.Note,
                fingerMachineDto.ConnectionType);

            var validateResult = await new UpdateFingerMachineValidator(_fingerMachineReadOnlyRepository, existFingerMachine).ValidateAsync(existFingerMachine, cancellationToken);

            if (!validateResult.IsValid)
            {
                NotifyValidationErrors(typeof(FingerMachine), validateResult.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }

            _fingerMachineWriteOnlyRepository.Update(existFingerMachine);
            await _fingerMachineWriteOnlyRepository.UnitOfWork.CommitAsync();
            return _mapper.Map<FingerMachineDto>(existFingerMachine);
        }
    }
}
