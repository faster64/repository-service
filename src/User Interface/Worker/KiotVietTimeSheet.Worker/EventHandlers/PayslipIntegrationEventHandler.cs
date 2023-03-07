using KiotVietTimeSheet.Application.EventBus.Events.PayslipEvents;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.AuditTrailWorker.EventHandlers
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
