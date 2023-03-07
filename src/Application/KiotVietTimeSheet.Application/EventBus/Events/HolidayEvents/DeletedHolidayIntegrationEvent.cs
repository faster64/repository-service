using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.HolidayEvents
{
    public class DeletedHolidayIntegrationEvent : IntegrationEvent
    {
        public Holiday Holiday { get; set; }

        [JsonConstructor]
        public DeletedHolidayIntegrationEvent()
        {

        }

        public DeletedHolidayIntegrationEvent(DeletedHolidayEvent @event)
        {
            Holiday = @event.Holiday;
        }
    }
}
