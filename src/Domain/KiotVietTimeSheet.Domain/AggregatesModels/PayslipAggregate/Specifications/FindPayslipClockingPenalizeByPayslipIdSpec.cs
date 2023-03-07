using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipClockingPenalizeByPayslipIdSpec : ExpressionSpecification<PayslipClockingPenalize>
    {
        public FindPayslipClockingPenalizeByPayslipIdSpec(long payslipId)
            : base(p => p.PayslipId == payslipId)
        {
        }
    }
}
