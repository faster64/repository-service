using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.EventBus.Events.PenalizeEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.DeletePenalizes
{
    public class DeletePenalizesCommandHandler : BaseCommandHandler,
        IRequestHandler<DeletePenalizesCommand, Unit>
    {
        private readonly IPenalizeWriteOnlyRepository _penalizeWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        public DeletePenalizesCommandHandler(
            IPenalizeWriteOnlyRepository penalizeWriteOnlyRepository,
            IEventDispatcher eventDispatcher,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService
        ) : base(eventDispatcher)
        {
            _penalizeWriteOnlyRepository = penalizeWriteOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }
        public async Task<Unit> Handle(DeletePenalizesCommand request, CancellationToken cancellationToken)
        {
            long id = request.Id;
            var existingPenalizes = await _penalizeWriteOnlyRepository.FindByIdAsync(id);
            if (existingPenalizes == null)
            {
                NotifyValidationErrors(typeof(Penalize), new List<string> { string.Format(Message.not_exists, Label.penalize) });
                return Unit.Value;
            }

            existingPenalizes.Delete();
            _penalizeWriteOnlyRepository.Delete(existingPenalizes);
            await _penalizeWriteOnlyRepository.UnitOfWork.CommitAsync();
            await _timeSheetIntegrationEventService.AddEventAsync(new DeletedPenalizeIntegrationEvent(existingPenalizes));
            return Unit.Value;
        }
    }
}
