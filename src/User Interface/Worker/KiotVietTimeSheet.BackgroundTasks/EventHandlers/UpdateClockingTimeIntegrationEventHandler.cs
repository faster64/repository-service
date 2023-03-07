using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.BackgroundTasks.EventHandlers
{
    public class UpdateClockingTimeIntegrationEventHandler : IIntegrationEventHandler<UpdateClockingTimeIntegrationEvent>
    {
        private readonly ILogger<UpdateClockingTimeIntegrationEvent> _logger;
        private readonly UpdateClockingProcess _updateClockingProcess;

        public UpdateClockingTimeIntegrationEventHandler(ILogger<UpdateClockingTimeIntegrationEvent> logger, UpdateClockingProcess updateClockingProcess)
        {
            _logger = logger;
            _updateClockingProcess = updateClockingProcess;
        }

        public async Task Handle(UpdateClockingTimeIntegrationEvent @event)
        {
            try
            {
                await _updateClockingProcess.UpdateClockingTimeAsync(@event);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
        }
    }
}