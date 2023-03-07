using System.Threading.Tasks;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.BackgroundTasks.EventHandlers
{
    public class PayslipPaymentIntegrationEventHandler :
        IIntegrationEventHandler<VoidedPayslipPaymentIntegrationEvent>,
        IIntegrationEventHandler<CreatedPayslipPaymentIntegrationEvent>
    {
        private readonly PayslipPaymentProcess _payslipPaymentProcess;

        public PayslipPaymentIntegrationEventHandler(PayslipPaymentProcess payslipPaymentProcess)
        {
            _payslipPaymentProcess = payslipPaymentProcess;
        }

        public async Task Handle(VoidedPayslipPaymentIntegrationEvent @event)
        {
            await _payslipPaymentProcess.VoidedPayslipTotalPaymentAsync(@event);
        }

        public async Task Handle(CreatedPayslipPaymentIntegrationEvent @event)
        {
            await _payslipPaymentProcess.CreatedPayslipPaymentAsync(@event);
        }
    }
}
