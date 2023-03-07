using System.Collections.Generic;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction
{
    public class DeductionRuleParam : IRuleParam
    {
        public List<DeductionParam> Deductions { get; set; }
    }
}
