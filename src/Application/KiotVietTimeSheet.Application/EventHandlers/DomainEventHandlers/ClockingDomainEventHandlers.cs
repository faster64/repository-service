
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Events;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Application.EventHandlers.DomainEventHandlers
{
    public class ClockingDomainEventHandlers :
        IEventHandler<SwappedClockingEvent>
    {
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        public ClockingDomainEventHandlers(ITimeSheetIntegrationEventService timeSheetIntegrationEventService)
        {
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task Handle(SwappedClockingEvent @notification, CancellationToken cancellationToken)
        {
            await _timeSheetIntegrationEventService.AddEventAsync(new SwappedClockingIntegrationEvent(@notification));
        }
    }
}
