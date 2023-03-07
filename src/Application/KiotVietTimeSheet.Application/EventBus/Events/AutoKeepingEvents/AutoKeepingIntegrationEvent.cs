using System;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.AutoKeepingEvents
{
    public class AutoKeepingIntegrationEvent : IntegrationEvent
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsRetry { get; set; }
        public long JobId { get; set; }

        [JsonConstructor]
        public AutoKeepingIntegrationEvent()
        {            
        }        
        public void SetEventId(Guid eventId)
        {
            this.Id = eventId;
        }

    }
}
