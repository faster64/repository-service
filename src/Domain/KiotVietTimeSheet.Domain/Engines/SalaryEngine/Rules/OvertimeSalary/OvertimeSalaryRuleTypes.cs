using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.OvertimeSalary
{
    public enum OvertimeSalaryRuleTypes
    {
        [Description("Theo giờ")]
        Hour = 1,
        [Description("Theo ca")]
        Shift = 2,
        [Description("Theo ngày")]
        Day = 3,
    }
}
