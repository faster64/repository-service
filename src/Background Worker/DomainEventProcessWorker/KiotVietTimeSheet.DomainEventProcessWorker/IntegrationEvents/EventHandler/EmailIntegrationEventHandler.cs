using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.SendMailEvents;
using KiotVietTimeSheet.DomainEventProcessWorker.SendMailProcesses;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;

namespace KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.EventHandler
{
    public class EmailIntegrationEventHandler :
        IIntegrationEventHandler<SentMailIntegrationEvent>

    {
        private readonly SendMailProcess _sendMailProcess;

        public EmailIntegrationEventHandler(SendMailProcess sendMailProcess)
        {
            _sendMailProcess = sendMailProcess;
        }

        public async Task Handle(SentMailIntegrationEvent @event)
        {
            await _sendMailProcess.SendMailAsync(@event);
        }


    }
}
