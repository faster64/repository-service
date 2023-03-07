using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Enum
{
    public enum SalaryPeriod
    {
        [Description("Hàng tháng")]
        EveryMonthly = 1,
        [Description("Hai lần một tháng")]
        TwiceMonthly = 2,
        [Description("Hàng tuần")]
        EveryWeekly = 3,
        [Description("Hai tuần một lần")]
        TwiceWeekly = 4,
        [Description("Tùy chọn")]
        Option = 5
    }
}
