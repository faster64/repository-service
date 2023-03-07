using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.Events
{
    public class RabbitPublishFailIntegrationEvent : IntegrationEvent
    {
        public RabbitPublishFailIntegrationEvent(string eventType, string eventData)
        {
            EventType = eventType;
            EventData = eventData;
        }

        public string EventType { get; set; }
        public string EventData { get; set; }
    }
}
