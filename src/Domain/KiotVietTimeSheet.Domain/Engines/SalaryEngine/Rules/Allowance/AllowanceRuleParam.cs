using System.Collections.Generic;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance
{
    public class AllowanceRuleParam : IRuleParam
    {
        public List<AllowanceParam> Allowances { get; set; }
    }
}
