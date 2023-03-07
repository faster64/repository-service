using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.EventBus.Events.HolidayEvents;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Events;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Application.EventHandlers.DomainEventHandlers
{
    public class HolidayDomainEventHandlers :
        IEventHandler<CreatedHolidayEvent>,
        IEventHandler<UpdatedHolidayEvent>,
        IEventHandler<DeletedHolidayEvent>
    {
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;

        public HolidayDomainEventHandlers(ITimeSheetIntegrationEventService timeSheetIntegrationEventService)
        {
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task Handle(CreatedHolidayEvent @notification, CancellationToken cancellationToken)
        {
            await _timeSheetIntegrationEventService.AddEventAsync(new CreatedHolidayIntegrationEvent(@notification));
        }

        public async Task Handle(UpdatedHolidayEvent @notification, CancellationToken cancellationToken)
        {
            await _timeSheetIntegrationEventService.AddEventAsync(new UpdatedHolidayIntegrationEvent(@notification));
        }

        public async Task Handle(DeletedHolidayEvent @notification, CancellationToken cancellationToken)
        {
            await _timeSheetIntegrationEventService.AddEventAsync(new DeletedHolidayIntegrationEvent(@notification));
        }
    }
}
