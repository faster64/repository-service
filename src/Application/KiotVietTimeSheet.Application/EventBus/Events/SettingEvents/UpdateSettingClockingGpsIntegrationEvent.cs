using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.SettingEvents
{
    public class UpdateSettingClockingGpsIntegrationEvent : IntegrationEvent
    {
        public bool UseClockingGps { get; set; }
        public UpdateSettingClockingGpsIntegrationEvent(bool useClockingGps)
        {
            UseClockingGps = useClockingGps;
        }

        [JsonConstructor]
        public UpdateSettingClockingGpsIntegrationEvent()
        {

        }
    }
}
