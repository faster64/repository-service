using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipClockingByPayslipIdSpec : ExpressionSpecification<PayslipClocking>
    {
        public FindPayslipClockingByPayslipIdSpec(long payslipId)
            : base(p => p.PayslipId == payslipId)
        {
        }
    }
}
