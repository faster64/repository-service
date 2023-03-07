using System.ComponentModel;

namespace KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Common
{
    /// <summary>
    /// Các giá trị enum phải tương ứng giống với cài đặt BunchOfEnum.FunctionType trong core
    /// </summary>
    public enum TimeSheetFunctionTypes
    { 
        [Description("Thiết lập cửa hàng")]
        PosParameter = 1,
        [Description("Quản lý Ca làm việc")]
        ShiftManagement = 200,
        [Description("Quản lý Lễ tết")]
        HolidayManagement = 201,
        [Description("Quản lý Nhân viên")]
        EmployeeManagement = 202,
        [Description("Quản lý Chi tiết ca làm việc")]
        TimeSheetManagement = 203,
        [Description("Quản lý Bảng lương")]
        PaysheetManagement = 204,
        [Description("Quản lý Phiếu lương")]
        PayslipManagement = 205,
        [Description("Đồng bộ dữ liệu chấm công")]
        AutoTimeKeeping = 206,
        [Description("Thiết lập hoa hồng")]
        CommissionManagement = 207,
        [Description("Thiết lập chung")]
        GeneralSettings = 209,
    }
    /// <summary>
    /// Các giá trị enum phải tương ứng giống với cài đặt BunchOfEnum.AuditTrailAction trong core
    /// </summary>
    public enum TimeSheetAuditTrailAction
    {
        [Description("Thêm mới")]
        Create = 1,
        [Description("Cập nhật")]
        Update = 2,
        [Description("Xóa")]
        Delete = 3,
        [Description("Hủy")]
        Reject = 4,
        [Description("Import")]
        ImportFile = 5,
        [Description("Export")]
        ExportFile = 6,
        [Description("Đổi ca")]

        SwapClocking = 200,
        [Description("Đồng bộ chấm công")]
        AutoTimekeeping = 201
    }
    /// <summary>
    /// Các giá trị enum phải tương ứng giống với cài đặt BunchOfEnum.AuditTrailAction trong core
    /// </summary>
    public enum TimeSheetSettingsTypes
    {
        [Description("Thiết lập chấm công")]
        Timekeeping = 1,
        [Description("Thiết lập tính lương")]
        Paysheet = 2,
        [Description("Thiết lập hoa hồng")]
        Commission = 3,
    }
}
