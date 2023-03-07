using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.HolidayEvents
{
    public class UpdatedHolidayIntegrationEvent : IntegrationEvent
    {
        public Holiday OldHoliday { get; set; }
        public Holiday NewHoliday { get; set; }

        [JsonConstructor]
        public UpdatedHolidayIntegrationEvent()
        {

        }

        public UpdatedHolidayIntegrationEvent(UpdatedHolidayEvent @event)
        {
            OldHoliday = @event.OldHoliday;
            NewHoliday = @event.NewHoliday;
        }
    }
}
