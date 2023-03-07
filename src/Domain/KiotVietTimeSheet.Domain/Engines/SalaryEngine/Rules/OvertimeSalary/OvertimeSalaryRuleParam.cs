using System.Collections.Generic;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.OvertimeSalary
{
    public class OvertimeSalaryRuleParam : IRuleParam
    {
        public List<OvertimeSalaryByShiftParam> OvertimeSalaryByShifts { get; set; }
    }
}
