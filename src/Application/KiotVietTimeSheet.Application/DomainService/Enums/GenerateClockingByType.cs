using System.ComponentModel;

namespace KiotVietTimeSheet.Application.DomainService.Enums
{
    public enum GenerateClockingByType
    {
        [Description("Ngày làm việc chi nhánh")]
        BranchWorkingDay = 1,
        [Description("Nghỉ lễ tết")]
        Holiday = 2,
        [Description("Lịch làm việc")]
        TimeSheet = 3
    }
}
