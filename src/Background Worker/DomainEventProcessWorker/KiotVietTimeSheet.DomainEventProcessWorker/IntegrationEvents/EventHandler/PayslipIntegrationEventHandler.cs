using KiotVietTimeSheet.Application.EventBus.Events.PayslipEvents;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.EventHandler
{
    public class PayslipIntegrationEventHandler :
        IIntegrationEventHandler<CancelPayslipIntegrationEvent>
    {
        private readonly PayslipAuditProcess _payslipAuditProcess;

        public PayslipIntegrationEventHandler(PayslipAuditProcess payslipAuditProcess)
        {
            _payslipAuditProcess = payslipAuditProcess;
        }

        public async Task Handle(CancelPayslipIntegrationEvent @event)
        {
            await _payslipAuditProcess.WriteCancelPayslipLogAsync(@event);
        }
    }
}
