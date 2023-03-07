using KiotVietTimeSheet.Application.EventBus.Events.SettingEvents;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.AuditTrailWorker.EventHandlers
{
    public class SettingIntegrationEventHandler :
        IIntegrationEventHandler<UpdatedSettingIntegrationEvent>
    {
        private readonly SettingAuditProcess _settingAuditProcess;

        public SettingIntegrationEventHandler(SettingAuditProcess settingAuditProcess)
        {
            _settingAuditProcess = settingAuditProcess;
        }

        public async Task Handle(UpdatedSettingIntegrationEvent @event)
        {
            await _settingAuditProcess.WriteUpdateSettingLogAsync(@event);
        }
    }
}
