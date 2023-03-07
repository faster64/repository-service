using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Enums
{
    public enum EmployeeTypeGet
    {
        [Description("Kiểu dữ liệu nhận biết từ web yêu cầu")]
        Web = 0,
        [Description("Kiểu dữ liệu nhận biết từ máy chấm công yêu cầu")]
        Attendance = 1
    }
}
