using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.SendMailEvents
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
