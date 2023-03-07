using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipByStatusSpec : ExpressionSpecification<Payslip>
    {
        public FindPayslipByStatusSpec(byte status)
            : base(p => p.PayslipStatus == status)
        { }
    }
}
