using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.OvertimeSalary
{
    public class OvertimeSalaryRuleValue : IRuleValue
    {
        public List<OvertimeSalaryDays> OvertimeSalaryDays { get; set; }
        public bool IsEqual(OvertimeSalaryRuleValue ruleValue)
        {
            return !OvertimeSalaryDays.Except(ruleValue.OvertimeSalaryDays).Any();
        }
    }
}
