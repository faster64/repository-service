using MediatR;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Application.Commands.DeleteFingerMachine
{
    public class DeleteFingerMachineCommandHandler : BaseCommandHandler,
        IRequestHandler<DeleteFingerMachineCommand, Unit>
    {
        private readonly IFingerMachineWriteOnlyRepository _fingerMachineWriteOnlyRepository;

        public DeleteFingerMachineCommandHandler(
            IEventDispatcher eventDispatcher,
            IFingerMachineWriteOnlyRepository fingerMachineWriteOnlyRepository

        ) : base(eventDispatcher)
        {
            _fingerMachineWriteOnlyRepository = fingerMachineWriteOnlyRepository;
        }

        public async Task<Unit> Handle(DeleteFingerMachineCommand request, CancellationToken cancellationToken)
        {
            long id = request.Id;
            var existFingerMachine = await _fingerMachineWriteOnlyRepository.FindByIdAsync(id);
            if (existFingerMachine != null)
                _fingerMachineWriteOnlyRepository.Delete(existFingerMachine);
            await _fingerMachineWriteOnlyRepository.UnitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
