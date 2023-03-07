using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents
{
    public class DeletedClockingIntegrationEvent : IntegrationEvent
    {
        public Clocking Clocking { get; set; }

        public DeletedClockingIntegrationEvent(DeletedClockingEvent @event)
        {
            Clocking = @event.Clocking;
        }

        [JsonConstructor]
        public DeletedClockingIntegrationEvent()
        {

        }
    }
}
