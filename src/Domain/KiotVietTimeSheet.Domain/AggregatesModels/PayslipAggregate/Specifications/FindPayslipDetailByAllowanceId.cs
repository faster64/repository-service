using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipDetailByAllowanceId : ExpressionSpecification<PayslipDetail>
    {
        public FindPayslipDetailByAllowanceId(long id)
            : base(p => p.RuleParam.Contains("\"" + nameof(AllowanceParam.AllowanceId) + "\":" + id + ',') && p.RuleType == nameof(AllowanceRule)) { }
    }
}
