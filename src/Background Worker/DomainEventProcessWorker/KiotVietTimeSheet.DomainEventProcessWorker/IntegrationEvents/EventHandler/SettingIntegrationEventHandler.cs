using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.SettingEvents;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;

namespace KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.EventHandler
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
