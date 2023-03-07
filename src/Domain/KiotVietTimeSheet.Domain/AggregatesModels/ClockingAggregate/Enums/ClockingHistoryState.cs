using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums
{
    public enum ClockingHistoryState
    {
        [Description("Đi muộn")]
        BeLateToWork = 1,
        [Description("Về sớm")]
        LeaveWorkEarly = 2,
        [Description("Làm thêm trước ca")]
        OverTimeBeforeShiftWork = 3,
        [Description("Làm thêm sau ca")]
        OverTimeAfterShiftWork = 4
    }
}
