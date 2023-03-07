using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Events
{
    public class CreatedPayslipEvent : DomainEvent
    {
        public List<Payslip> Payslips { get; set; }

        public CreatedPayslipEvent(List<Payslip> payslips)
        {
            Payslips = payslips;
        }
    }
}
