using System;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.BackgroundTasks.EventHandlers
{
    public class AutoGenerateClockingIntegrationEventHandler : 
        IIntegrationEventHandler<CreateAutoGenerateClockingIntegrationEvent>
    {
        private readonly ILogger<CreateAutoGenerateClockingIntegrationEvent> _logger;
        private readonly AutoGenerateClockingProcess _autoGenerateClockingProcess;

        public AutoGenerateClockingIntegrationEventHandler(
            ILogger<CreateAutoGenerateClockingIntegrationEvent> logger,
            AutoGenerateClockingProcess autoGenerateClockingProcess)
        {
            _logger = logger;
            _autoGenerateClockingProcess = autoGenerateClockingProcess;
        }

        public async Task Handle(CreateAutoGenerateClockingIntegrationEvent @event)
        {
            try
            {
                await _autoGenerateClockingProcess.Create(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
