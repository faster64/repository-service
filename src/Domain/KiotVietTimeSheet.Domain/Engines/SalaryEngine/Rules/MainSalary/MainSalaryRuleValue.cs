using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary
{
    public class MainSalaryRuleValue : IRuleValue
    {
        public MainSalaryTypes Type { get; set; }
        public List<MainSalaryRuleValueDetail> MainSalaryValueDetails { get; set; }

        public bool IsEqual(MainSalaryRuleValue ruleValue)
        {
            if (ruleValue == null) return false;
            if (Type != ruleValue.Type) return false;
            if (MainSalaryValueDetails == null && ruleValue.MainSalaryValueDetails == null) return true;
            return MainSalaryValueDetails?.Count == ruleValue.MainSalaryValueDetails?.Count &&
                   MainSalaryValueDetails.TrueForAll(detail => ruleValue.MainSalaryValueDetails.Any(detail.IsEqual));
        }
    }
}
