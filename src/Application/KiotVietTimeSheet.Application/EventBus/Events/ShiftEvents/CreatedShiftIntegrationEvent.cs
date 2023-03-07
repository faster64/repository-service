using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.ShiftEvents
{
    public class CreatedShiftIntegrationEvent : IntegrationEvent
    {
        public Shift Shift { get; set; }
        public bool IsGeneralSetting { get; set; }

        [JsonConstructor]
        public CreatedShiftIntegrationEvent()
        {

        }

        public CreatedShiftIntegrationEvent(CreatedShiftEvent @event)
        {
            Shift = @event.Shift;
            IsGeneralSetting = @event.IsGeneralSetting;
        }
    }
}
