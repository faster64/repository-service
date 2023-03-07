using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents
{
    public class UpdatedClockingIntegrationEvent : IntegrationEvent
    {
        public Clocking OldClocking { get; set; }
        public Clocking NewClocking { get; set; }

        public UpdatedClockingIntegrationEvent(UpdatedClockingEvent @event)
        {
            OldClocking = @event.OldClocking;
            NewClocking = @event.NewClocking;
        }

        [JsonConstructor]
        public UpdatedClockingIntegrationEvent()
        {

        }
    }
}
