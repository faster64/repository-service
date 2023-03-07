using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum
{
    public enum PaysheetStatuses
    {
        [Description("Đã hủy")]
        Void = 0,
        [Description("Tạm tính")]
        TemporarySalary = 1,
        [Description("Đã chốt lương")]
        PaidSalary = 2,
        [Description("Nháp")]
        Draft = 3,
        [Description("Đang tạo")]
        Pending = 4
    }

    public enum PaysheetProcessCompleteStep
    {
        [Description("Số step hoàn thành")]
        StepNumberComplete = 5
    }
    public enum PaysheetErrorCode
    {
        [Description("Bảng hoa hồng")]
        Commision = 1,
        [Description("Danh sách thiết lập lương cho nhân viên")]
        Payrate = 2,
        [Description("Danh sách phụ cấp")]
        Allowance = 3,
        [Description("Danh sách giảm trừ")]
        Deduction = 4,
        [Description("Danh sách chấm công")]
        Clocking = 5,
        [Description("Doanh thu cho nhân viên")]
        Revenue = 6,
        [Description("Danh sách nhân viên")]
        Employee = 7
    }
}
 