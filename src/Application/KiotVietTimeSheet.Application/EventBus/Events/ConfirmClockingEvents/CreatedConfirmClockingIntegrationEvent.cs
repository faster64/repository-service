using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.ConfirmClockingEvents
{
    public class CreatedConfirmClockingIntegrationEvent : IntegrationEvent
    {
        [JsonConstructor]
        public CreatedConfirmClockingIntegrationEvent()
        {
        }
    }
}
