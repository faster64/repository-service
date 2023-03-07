using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipByPaysheetIdSpec : ExpressionSpecification<Payslip>
    {
        public FindPayslipByPaysheetIdSpec(long paysheetId)
            : base(p => p.PaysheetId == paysheetId)
        {
        }
    }
}
