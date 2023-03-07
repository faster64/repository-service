using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class GetPayslipByPaysheetId : ExpressionSpecification<Payslip>
    {
        public GetPayslipByPaysheetId(long paysheetId)
           : base(e => e.PaysheetId == paysheetId)
        { }
    }
}
