using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums
{
    public enum ClockingStatuses
    {
        [Description("Đã hủy")]
        Void = 0,
        [Description("Chưa chấm công")]
        Created = 1,
        [Description("Đã chấm công vào")]
        CheckedIn = 2,
        [Description("Đã chấm công ra")]
        CheckedOut = 3,
        [Description("Nghỉ")]
        WorkOff = 4
    }
}
