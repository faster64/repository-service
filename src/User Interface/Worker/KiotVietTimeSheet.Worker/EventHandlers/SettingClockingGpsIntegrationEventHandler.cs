using KiotVietTimeSheet.Application.EventBus.Events.SettingEvents;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.AuditTrailWorker.EventHandlers
{
    public class SettingClockingGpsIntegrationEventHandler :
        IIntegrationEventHandler<UpdateSettingClockingGpsIntegrationEvent>
    {
        private readonly SettingAuditProcess _settingAuditProcess;

        public SettingClockingGpsIntegrationEventHandler(SettingAuditProcess settingAuditProcess)
        {
            _settingAuditProcess = settingAuditProcess;
        }

        public async Task Handle(UpdateSettingClockingGpsIntegrationEvent @event)
        {
            await _settingAuditProcess.WriteUpdateSettingClockingGpsLogAsync(@event);
        }
    }
}
