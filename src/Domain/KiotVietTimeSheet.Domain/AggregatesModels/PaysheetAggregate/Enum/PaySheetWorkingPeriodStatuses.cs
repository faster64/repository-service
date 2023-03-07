using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum
{
    public enum PaySheetWorkingPeriodStatuses
    {
        [Description("Hàng tháng")]
        EveryMonth = 1,
        [Description("Hai lần một tháng")]
        TwiceAMonth = 2,
        [Description("Hàng tuần")]
        EveryWeek = 3,
        [Description("Hai tuần một lần")]
        TwiceWeekly = 4,
        [Description("Tùy chọn")]
        Option = 5
    }
}
