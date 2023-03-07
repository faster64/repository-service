using System;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.ConfirmClockingEvents;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.BackgroundTasks.EventHandlers
{
    public class ConfirmClockingIntegrationEventHandler :
        IIntegrationEventHandler<CreatedConfirmClockingIntegrationEvent>
    {
        private readonly ConfirmClockingProcess _confirmClockingProcess;
        private readonly ILogger<CreatedConfirmClockingIntegrationEvent> _logger;

        public ConfirmClockingIntegrationEventHandler(ConfirmClockingProcess confirmClockingProcess, ILogger<CreatedConfirmClockingIntegrationEvent> logger)
        {
            _confirmClockingProcess = confirmClockingProcess;
            _logger = logger;
        }

        public async Task Handle(CreatedConfirmClockingIntegrationEvent @event)
        {
            
            try
            {
                await _confirmClockingProcess.CreateConfirmClockingAsync(@event);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
        }
    }
}
