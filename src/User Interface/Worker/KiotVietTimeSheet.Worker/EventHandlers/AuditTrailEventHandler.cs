
using KiotVietTimeSheet.Application.EventBus.Events.AutoTimeKeepingEvents;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using System;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.AuditTrailWorker.EventHandlers
{
    public class AuditTrailEventHandler : IIntegrationEventHandler<AutoTimeKeepingIntegrationEvent>
    {
        private readonly IEventContextService _eventContextService;
        public AuditTrailEventHandler(IEventContextService eventContextService)
        {
            _eventContextService = eventContextService;
        }
        public Task Handle(AutoTimeKeepingIntegrationEvent @event)
        {
            Console.WriteLine(_eventContextService.Context);
            return Task.CompletedTask;
        }
    }
}
