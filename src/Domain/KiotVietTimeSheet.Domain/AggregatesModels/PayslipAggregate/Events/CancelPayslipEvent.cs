using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Events
{
    public class CancelPayslipEvent : DomainEvent
    {
        public Payslip Payslip { get; set; }

        public CancelPayslipEvent(Payslip payslip)
        {
            Payslip = payslip;
        }
    }
}
