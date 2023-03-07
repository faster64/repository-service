using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.PayRateTemplateEvents;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;

namespace KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.EventHandler
{
    public class PayRateTemplateIntegrationEventHandler :
        IIntegrationEventHandler<CreatedPayRateTemplateIntegrationEvent>,
        IIntegrationEventHandler<UpdatedPayRateTemplateIntegrationEvent>,
        IIntegrationEventHandler<DeletedPayRateTemplateIntegrationEvent>
    {
        private readonly PayRateTemplateAuditProcess _payRateTemplateAuditProcess;

        public PayRateTemplateIntegrationEventHandler(PayRateTemplateAuditProcess payRateTemplateAuditProcess)
        {
            _payRateTemplateAuditProcess = payRateTemplateAuditProcess;
        }

        public async Task Handle(CreatedPayRateTemplateIntegrationEvent @event)
        {
            await _payRateTemplateAuditProcess.WriteCreatePayRateTemplateLogAsync(@event);
        }

        public async Task Handle(UpdatedPayRateTemplateIntegrationEvent @event)
        {
            await _payRateTemplateAuditProcess.WriteUpdatePayRateTemplateLogAsync(@event);
        }

        public async Task Handle(DeletedPayRateTemplateIntegrationEvent @event)
        {
            await _payRateTemplateAuditProcess.WriteDeletePayRateTemplateLogAsync(@event);
        }
    }
}
