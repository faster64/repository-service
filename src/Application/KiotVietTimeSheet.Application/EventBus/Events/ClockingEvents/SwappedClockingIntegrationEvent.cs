using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents
{
    public class SwappedClockingIntegrationEvent : IntegrationEvent
    {
        public Clocking OldClocking { get; set; }
        public Clocking NewClocking { get; set; }

        public SwappedClockingIntegrationEvent(SwappedClockingEvent @event)
        {
            OldClocking = @event.OldClocking;
            NewClocking = @event.NewClocking;
        }

        [JsonConstructor]
        public SwappedClockingIntegrationEvent()
        {

        }
    }
}
