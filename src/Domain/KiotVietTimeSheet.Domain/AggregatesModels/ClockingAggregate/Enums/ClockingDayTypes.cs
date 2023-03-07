using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums
{
    public enum ClockingDayTypes
    {
        [Description("Ngày làm việc bình thường")]
        Normal = 1,
        [Description("Ngày nghỉ chi nhánh")]
        DayOff = 2,
        [Description("Nghỉ lễ")]
        Holiday = 3
    }
}
