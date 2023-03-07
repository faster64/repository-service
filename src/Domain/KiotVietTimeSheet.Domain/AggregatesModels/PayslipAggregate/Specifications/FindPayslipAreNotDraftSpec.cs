using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipAreNotDraftSpec : ExpressionSpecification<Payslip>
    {
        public FindPayslipAreNotDraftSpec()
            : base(p => !p.IsDraft)
        { }
    }
}
