using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.ShiftEvents
{
    public class UpdatedShiftIntegrationEvent : IntegrationEvent
    {
        public Shift OldShift { get; set; }
        public Shift Shift { get; set; }
        public bool IsGeneralSetting { get; set; }

        [JsonConstructor]
        public UpdatedShiftIntegrationEvent()
        {

        }

        public UpdatedShiftIntegrationEvent(UpdatedShiftEvent @event)
        {
            OldShift = @event.OldShift;
            Shift = @event.Shift;
            IsGeneralSetting = @event.IsGeneralSetting;
        }
    }
}
