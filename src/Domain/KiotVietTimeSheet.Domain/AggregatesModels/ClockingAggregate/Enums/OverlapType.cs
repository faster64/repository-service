using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums
{
    public enum OverlapType
    {
        [Description("Trùng lịch làm việc")]
        TimeSheet = 1,
        [Description("Trùng ngày nghỉ chi nhánh")]
        BranchOffDay = 2,
        [Description("Trùng nghỉ lễ")]
        Holiday = 3
    }
}
