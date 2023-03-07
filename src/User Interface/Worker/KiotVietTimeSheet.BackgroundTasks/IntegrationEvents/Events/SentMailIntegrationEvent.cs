using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.BackgroundTasks.IntegrationEvents.Events
{
    public class SentMailIntegrationEvent : IntegrationEvent
    {
        public EmailEvent EmailEvent { get; set; }

        public SentMailIntegrationEvent(EmailEvent emailEvent)
        {
            EmailEvent = emailEvent;
        }

        [JsonConstructor]
        public SentMailIntegrationEvent()
        {

        }
    }
}
