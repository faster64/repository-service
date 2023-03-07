using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipByIdSpec : ExpressionSpecification<Payslip>
    {
        public FindPayslipByIdSpec(long id)
            : base(p => p.Id == id)
        {
        }
    }
}
