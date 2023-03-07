using KiotVietTimeSheet.Application.EventBus.Events.AutoTimeKeepingEvents;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.AuditTrailWorker.EventHandlers
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
