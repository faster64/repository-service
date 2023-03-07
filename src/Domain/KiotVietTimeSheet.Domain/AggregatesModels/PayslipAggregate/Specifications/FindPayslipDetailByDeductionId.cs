using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipDetailByDeductionId : ExpressionSpecification<PayslipDetail>
    {
        public FindPayslipDetailByDeductionId(long id)
            : base(p => p.RuleParam.Contains("\"" + nameof(DeductionParam.DeductionId) + "\":" + id + ",") && p.RuleType == nameof(DeductionRule)) { }
    }
}
