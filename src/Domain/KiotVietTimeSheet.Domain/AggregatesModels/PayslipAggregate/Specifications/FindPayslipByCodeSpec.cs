using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipByCodeSpec : ExpressionSpecification<Payslip>
    {
        public FindPayslipByCodeSpec(string code)
            : base(e => e.Code.Trim().ToLower() == code.Trim().ToLower())
        { }
    }
}
