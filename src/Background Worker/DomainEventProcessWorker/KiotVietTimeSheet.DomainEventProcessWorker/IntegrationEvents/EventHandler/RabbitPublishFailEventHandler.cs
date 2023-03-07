using System.Threading.Tasks;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.Events;
using System.Linq;
using System.Reflection;

namespace KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.EventHandler
{
    public class RabbitPublishFailEventHandler : IIntegrationEventHandler<RabbitPublishFailIntegrationEvent>
    {
        private readonly IEventBus _eventBus;

        public RabbitPublishFailEventHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public Task Handle(RabbitPublishFailIntegrationEvent @event)
        {
            var eventType = Assembly.GetAssembly(typeof(RabbitPublishFailIntegrationEvent))
                .GetTypes()
                .FirstOrDefault(t => t.Name == @event.EventType);

            _eventBus.Publish(eventType, @event.EventData);

            return Task.CompletedTask;
        }
    }
}
