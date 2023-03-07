namespace KiotVietTimeSheet.SharedKernel.EventBus
{
    public class CreatedPayslipPaymentIntegrationEvent: IntegrationEvent
    {
        public CreatedPayslipPaymentIntegrationEvent(long payslipId, decimal amount)
        {
            PayslipId = payslipId;
            Amount = amount;
        }

        public long PayslipId { get; set; }
        public decimal Amount { get; set; }
    }
}