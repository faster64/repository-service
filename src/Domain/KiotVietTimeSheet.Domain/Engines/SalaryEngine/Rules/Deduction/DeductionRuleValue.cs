using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction
{
    public class DeductionRuleValue : IRuleValue
    {
        public List<DeductionRuleValueDetail> DeductionRuleValueDetails { get; set; }
        public bool IsEqual(DeductionRuleValue ruleValue)
        {
            if (ruleValue == null) return false;
            if (DeductionRuleValueDetails == null && ruleValue.DeductionRuleValueDetails == null) return true;
            return DeductionRuleValueDetails?.Count == ruleValue.DeductionRuleValueDetails?.Count &&
                   DeductionRuleValueDetails.TrueForAll(detail => ruleValue.DeductionRuleValueDetails.Any(detail.IsEqual));
        }
    }
}
