using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.EventBus.Events.ShiftEvents;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Events;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Application.EventHandlers.DomainEventHandlers
{
    public class ShiftDomainEventHandlers :
        IEventHandler<CreatedShiftEvent>,
        IEventHandler<UpdatedShiftEvent>,
        IEventHandler<DeletedShiftEvent>
    {
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        public ShiftDomainEventHandlers(ITimeSheetIntegrationEventService timeSheetIntegrationEventService)
        {
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task Handle(CreatedShiftEvent @notification, CancellationToken cancellationToken)
        {
            await _timeSheetIntegrationEventService.AddEventAsync(new CreatedShiftIntegrationEvent(@notification));
        }

        public async Task Handle(UpdatedShiftEvent @notification, CancellationToken cancellationToken)
        {
            await _timeSheetIntegrationEventService.AddEventAsync(new UpdatedShiftIntegrationEvent(@notification));
        }

        public async Task Handle(DeletedShiftEvent @notification, CancellationToken cancellationToken)
        {
            await _timeSheetIntegrationEventService.AddEventAsync(new DeletedShiftIntegrationEvent(@notification));
        }
    }
}
