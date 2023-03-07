using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.HolidayEvents
{
    public class CreatedHolidayIntegrationEvent : IntegrationEvent
    {
        public Holiday Holiday { get; set; }

        [JsonConstructor]
        public CreatedHolidayIntegrationEvent()
        {

        }

        public CreatedHolidayIntegrationEvent(CreatedHolidayEvent @event)
        {
            Holiday = @event.Holiday;
        }
    }
}
