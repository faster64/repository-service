namespace KiotVietTimeSheet.SharedKernel.EventBus
{
    public class VoidedPayslipPaymentIntegrationEvent : IntegrationEvent
    {
        public VoidedPayslipPaymentIntegrationEvent(long payslipId, decimal amount)
        {
            PayslipId = payslipId;
            Amount = amount;
        }

        public long PayslipId { get; set; }
        public decimal Amount { get; set; }
    }
}