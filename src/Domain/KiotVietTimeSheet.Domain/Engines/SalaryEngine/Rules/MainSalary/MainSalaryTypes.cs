using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary
{
    public enum MainSalaryTypes
    {
        [Description("Theo giờ")]
        Hour = 1,
        [Description("Theo ca")]
        Shift = 2,
        [Description("Theo ngày")]
        Day = 3,
        [Description("Cố định")]
        Fixed = 4
    }
}
