using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommissionSalary
{
    public class CommissionSalaryRuleValue : IRuleValue
    {
        public decimal? MinCommission { get; set; }
        public bool UseMinCommission { get; set; }
        public List<CommissionSalaryRuleValueDetail> CommissionSalaryRuleValueDetails { get; set; }

        public bool IsEqual(CommissionSalaryRuleValue ruleValue)
        {
            if (ruleValue == null) return false;
            if (UseMinCommission != ruleValue.UseMinCommission || MinCommission != ruleValue.MinCommission) return false;
            if (CommissionSalaryRuleValueDetails == null && ruleValue.CommissionSalaryRuleValueDetails == null) return true;
            return CommissionSalaryRuleValueDetails?.Count == ruleValue.CommissionSalaryRuleValueDetails?.Count &&
                   CommissionSalaryRuleValueDetails.TrueForAll(detail => ruleValue.CommissionSalaryRuleValueDetails.Any(detail.IsEqual));
        }
    }
}
