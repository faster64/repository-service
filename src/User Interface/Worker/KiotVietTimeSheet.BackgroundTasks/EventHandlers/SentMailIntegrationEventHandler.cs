using System.Threading.Tasks;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types;
using KiotVietTimeSheet.BackgroundTasks.IntegrationEvents.Events;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;

namespace KiotVietTimeSheet.BackgroundTasks.EventHandlers
{
    public class SentMailIntegrationEventHandler : IIntegrationEventHandler<SentMailIntegrationEvent>
    {
        private readonly SendMailProcess _sendMailProcess;
        public SentMailIntegrationEventHandler(
            ILogger<SentMailIntegrationEventHandler> logger,
            SendMailProcess sendMailProcess)
        {
            _sendMailProcess = sendMailProcess;
        }

        public async Task Handle(SentMailIntegrationEvent @event)
        {
            await _sendMailProcess.SendMailIntegration(@event);
        }
    }
}
