using KiotVietTimeSheet.Application.EventBus.Events.AutoKeepingEvents;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.BackgroundTasks.EventHandlers
{
    public class AutoKeepingIntegrationEventHandler : IIntegrationEventHandler<AutoKeepingIntegrationEvent>
    {
        private readonly AutoKeepingProcess _autoKeepingProcess;
        public AutoKeepingIntegrationEventHandler(AutoKeepingProcess autoKeepingProcess)
        {
            _autoKeepingProcess = autoKeepingProcess;
        }

        public async Task Handle(AutoKeepingIntegrationEvent @event)
        {
            await _autoKeepingProcess.AutoDailyKeepingAsync(@event);
        }
    }
}