using KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.AuditTrailWorker.EventHandlers
{
    public class PaysheetIntegrationEventHandler :
        IIntegrationEventHandler<CreatedPaysheetIntegrationEvent>,
        IIntegrationEventHandler<UpdatedPaysheetIntegrationEvent>,
        IIntegrationEventHandler<CancelPaysheetIntegrationEvent>,
        IIntegrationEventHandler<UpdatePaysheetProcessIntegrationEvent>,
        IIntegrationEventHandler<UpdatePaysheetProcessErrorIntegrationEvent>,
        IIntegrationEventHandler<CreatePaysheetProcessIntegrationEvent>
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

        public async Task Handle(UpdatePaysheetProcessIntegrationEvent @event)
        {
            await _paysheetAuditProcess.WriteUpdatePaysheetProcessLogAsync(@event);
        }

        public async Task Handle(UpdatePaysheetProcessErrorIntegrationEvent @event)
        {
            await _paysheetAuditProcess.WriteUpdatePaysheetProcessErrorLogAsync(@event);
        }

        public async Task Handle(CreatePaysheetProcessIntegrationEvent @event)
        {
            await _paysheetAuditProcess.WriteCreatePaysheetProcessLogAsync(@event);
        }
    }
}
