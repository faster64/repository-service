using System.Threading.Tasks;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types;
using KiotVietTimeSheet.BackgroundTasks.IntegrationEvents.Events;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;

namespace KiotVietTimeSheet.BackgroundTasks.EventHandlers
{
    public class ActivedFeatureIntegrationEventHandler : IIntegrationEventHandler<ActivedFeatureIntegrationEvent>
    {
        private readonly ActiveTimesheetProcess _activeTimesheetProcess;
        public ActivedFeatureIntegrationEventHandler(
            ILogger<ActivedFeatureIntegrationEventHandler> logger,
            ActiveTimesheetProcess activeTimesheetProcess)
        {
            _activeTimesheetProcess = activeTimesheetProcess;
        }

        public async Task Handle(ActivedFeatureIntegrationEvent @event)
        {
            await _activeTimesheetProcess.ActivedFeatureIntegration(@event);
        }
    }
}
