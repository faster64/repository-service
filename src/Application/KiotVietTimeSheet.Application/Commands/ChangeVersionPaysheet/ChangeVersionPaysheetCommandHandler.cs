using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.ChangeVersionPaysheet
{
    public class ChangeVersionPaysheetCommandHandler : BaseCommandHandler,
        IRequestHandler<ChangeVersionPaysheetCommand, Unit>
    {
        private readonly IPaySheetOutOfDateDomainService _paySheetOutOfDateDomainService;
        private readonly IPaysheetWriteOnlyRepository _paysheetWriteOnlyRepository;

        public ChangeVersionPaysheetCommandHandler(
            IEventDispatcher eventDispatcher,
            IPaySheetOutOfDateDomainService paySheetOutOfDateDomainService,
            IPaysheetWriteOnlyRepository paysheetWriteOnlyRepository
        )
            : base(eventDispatcher)
        {
            _paySheetOutOfDateDomainService = paySheetOutOfDateDomainService;
            _paysheetWriteOnlyRepository = paysheetWriteOnlyRepository;
        }

        public async Task<Unit> Handle(ChangeVersionPaysheetCommand request, CancellationToken cancellationToken)
        {
            await _paySheetOutOfDateDomainService.WithPaysheetChangeAsync(request.Ids);
            await _paysheetWriteOnlyRepository.UnitOfWork.CommitAsync();
            return Unit.Value;
        }

    }
}
