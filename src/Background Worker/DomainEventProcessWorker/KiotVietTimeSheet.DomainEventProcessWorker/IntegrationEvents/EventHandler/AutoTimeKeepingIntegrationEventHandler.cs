using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.AutoTimeKeepingEvents;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;

namespace KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.EventHandler
{
    public class AutoTimeKeepingIntegrationEventHandler : IIntegrationEventHandler<AutoTimeKeepingIntegrationEvent>
    {
        private readonly AutoTimeKeepingAuditProcess _autoTimeKeepingAuditProcess;

        public AutoTimeKeepingIntegrationEventHandler(AutoTimeKeepingAuditProcess autoTimeKeepingAuditProcess)
        {
            _autoTimeKeepingAuditProcess = autoTimeKeepingAuditProcess;
        }
        public async Task Handle(AutoTimeKeepingIntegrationEvent @event)
        {
            await _autoTimeKeepingAuditProcess.WriteAutoTimekeepingAudit(@event);
        }
    }
}
