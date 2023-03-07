using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums
{
    public enum ClockingHistoryStatuses
    {
        [Description("Hủy")]
        Void = 0,
        [Description("Hoạt động")]
        Created = 1
    }
}
