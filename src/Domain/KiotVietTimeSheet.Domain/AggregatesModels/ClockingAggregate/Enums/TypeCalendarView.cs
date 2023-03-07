using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums
{
    public enum TypeCalendarView
    {
        [Description("Hiển thị theo tháng hiện tại")]
        KvTimeLineMonth = 0,
        [Description("Hiển thị theo tuần hiện tại")]
        KvTimeLineWeek = 1,
        [Description("Hiển thị theo ngày hiện tại")]
        KvTimeLineDay = 2,
        [Description("Không dùng để hiển thị")]
        KvTimeLine = 3
    }
}
