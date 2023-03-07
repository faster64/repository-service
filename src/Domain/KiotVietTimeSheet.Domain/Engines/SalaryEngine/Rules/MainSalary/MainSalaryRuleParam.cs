using System.Collections.Generic;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary
{
    public class MainSalaryRuleParam : IRuleParam
    {
        public List<MainSalaryByShiftParam> MainSalaryShifts { get; set; }
    }
}
