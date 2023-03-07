using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance
{
    public class AllowanceRuleValue : IRuleValue
    {
        public List<AllowanceRuleValueDetail> AllowanceRuleValueDetails { get; set; }
        public bool IsEqual(AllowanceRuleValue ruleValue)
        {
            if (ruleValue == null) return false;
            if (AllowanceRuleValueDetails == null && ruleValue.AllowanceRuleValueDetails == null) return true;
            return AllowanceRuleValueDetails?.Count == ruleValue.AllowanceRuleValueDetails?.Count &&
                   AllowanceRuleValueDetails.TrueForAll(detail => ruleValue.AllowanceRuleValueDetails.Any(detail.IsEqual));
        }
    }
}
