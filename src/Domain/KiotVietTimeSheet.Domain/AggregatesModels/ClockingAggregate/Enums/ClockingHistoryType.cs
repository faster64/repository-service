using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums
{
    public enum ClockingHistoryType
    {
        [Description("Hôm trước")] DayBefore = 1,
        [Description("Đúng ngày")] Today = 2,
        [Description("Hôm sau")] DayAfter = 3,
    }
}
