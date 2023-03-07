using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UpdatePaysheets
{
    public class UpdatePaysheetsCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdatePaysheetsCommand, Unit>
    {
        private readonly IPaysheetWriteOnlyRepository _paysheetWriteOnlyRepository;

        public UpdatePaysheetsCommandHandler(
            IEventDispatcher eventDispatcher,
            IPaysheetWriteOnlyRepository paysheetWriteOnlyRepository
        )
            : base(eventDispatcher)
        {
            _paysheetWriteOnlyRepository = paysheetWriteOnlyRepository;
        }

        public async Task<Unit> Handle(UpdatePaysheetsCommand request, CancellationToken cancellationToken)
        {
            _paysheetWriteOnlyRepository.BatchUpdate(request.Paysheets);
            await _paysheetWriteOnlyRepository.UnitOfWork.CommitAsync();
            return Unit.Value;
        }
    }
}
