using KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.EventHandler
{
    public class PaysheetIntegrationEventHandler :
        IIntegrationEventHandler<CreatedPaysheetIntegrationEvent>,
        IIntegrationEventHandler<UpdatedPaysheetIntegrationEvent>,
        IIntegrationEventHandler<CancelPaysheetIntegrationEvent>
    {
        private readonly PaysheetAuditProcess _paysheetAuditProcess;

        public PaysheetIntegrationEventHandler(PaysheetAuditProcess paysheetAuditProcess)
        {
            _paysheetAuditProcess = paysheetAuditProcess;
        }

        public async Task Handle(CreatedPaysheetIntegrationEvent @event)
        {
            await _paysheetAuditProcess.WriteCreatePaysheetLogAsync(@event);
        }

        public async Task Handle(UpdatedPaysheetIntegrationEvent @event)
        {
            await _paysheetAuditProcess.WriteUpdatePaysheetLogAsync(@event);
        }

        public async Task Handle(CancelPaysheetIntegrationEvent @event)
        {
            await _paysheetAuditProcess.WriteCancelPaysheetLogAsync(@event);
        }
    }
}
