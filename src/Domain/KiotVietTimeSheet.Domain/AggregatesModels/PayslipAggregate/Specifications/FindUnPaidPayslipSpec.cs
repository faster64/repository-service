using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindUnPaidPayslipSpec : ExpressionSpecification<Payslip>
    {
        public FindUnPaidPayslipSpec()
            : base(e => e.TotalPayment < e.NetSalary)
        { }
    }
}
