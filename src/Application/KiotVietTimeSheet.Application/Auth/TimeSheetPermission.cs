using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Auth
{
    public class TimeSheetPermission
    {
        public const string _Read = "_Read";
        public const string _Create = "_Create";
        public const string _Update = "_Update";
        public const string _Delete = "_Delete";

        protected TimeSheetPermission()
        {
        }

        [PermissionMeta(Name = "Sao chép")]
        public const string Clocking_Copy = "Clocking_Copy";
        [PermissionMeta(Name = "Xem DS", Parents = new[] { FingerPrintLog_Read, Clocking_Copy, Branch_Read, Employee_Delete, Paysheet_Read, Paysheet_Create, Paysheet_Complete, FingerPrintLog_Read })]
        public const string Clocking_Read = "Clocking_Read";
        [PermissionMeta(Name = "Tạo mới", Parents = new[] { Clocking_Update, Clocking_Copy, Branch_Create, Branch_Update })]
        public const string Clocking_Create = "Clocking_Create";
        // Đây là quyền hủy lịch làm việc. Vì hệ thống hiện tại đang để quyền Xóa tương ứng với quyền hủy nên BA mong muốn vẫn đặt tên là xóa
        [PermissionMeta(Name = "Xóa", Parents = new[] { Branch_Create, Branch_Update })]
        public const string Clocking_Delete = "Clocking_Delete";
        [PermissionMeta(Name = "Sửa", Parents = new[] { FingerPrintLog_Read, Clocking_Copy, Branch_Create, Branch_Update, Clocking_Delete, Employee_Delete, Paysheet_Update, Paysheet_Complete, Paysheet_Delete, FingerPrintLog_Read })]
        public const string Clocking_Update = "Clocking_Update";
        [PermissionMeta(Name = "Xuấy file")]
        public const string Clocking_Export = "Clocking_Export";

        [PermissionMeta(Name = "Xem DS", Children = new[] { Clocking_Read, Clocking_Create, Clocking_Update, Clocking_Copy, Branch_Read, Employee_Delete })]
        public const string TimeSheet_Read = "TimeSheet_Read";
        [PermissionMeta(Name = "Tạo mới", Children = new[] { Clocking_Create, Clocking_Update, Clocking_Copy })]
        public const string TimeSheet_Create = "TimeSheet_Create";
        [PermissionMeta(Name = "Cập nhật", Children = new[] { Clocking_Update, Branch_Create, Branch_Update, Clocking_Delete, Employee_Delete })]
        public const string TimeSheet_Update = "TimeSheet_Update";
        [PermissionMeta(Name = "Xóa", Parents = new[] { Clocking_Delete })]
        public const string TimeSheet_Delete = "TimeSheet_Delete";


        [PermissionMeta(Name = "Xem DS", Children = new[] { Clocking_Read, Clocking_Create, Clocking_Update, Clocking_Copy, Branch_Create, Branch_Update })]
        public const string TimeSheetShift_Read = "TimeSheetShift_Read";
        [PermissionMeta(Name = "Tạo mới", Children = new[] { Clocking_Create, Clocking_Update, Clocking_Copy, Branch_Create, Branch_Update })]
        public const string TimeSheetShift_Create = "TimeSheetShift_Create";
        [PermissionMeta(Name = "Cập nhật", Children = new[] { Clocking_Create, Clocking_Update, Clocking_Copy, Branch_Create, Branch_Update })]
        public const string TimeSheetShift_Update = "TimeSheetShift_Update";
        [PermissionMeta(Name = "Xóa", Parents = new[] { Clocking_Create, Clocking_Update, Clocking_Copy, Branch_Create, Branch_Update })]
        public const string TimeSheetShift_Delete = "TimeSheetShift_Delete";

        [PermissionMeta(Name = "Xem DS", Parents = new[] { FingerPrintLog_Read, Clocking_Read, Clocking_Update })]
        public const string ClockingHistory_Read = "ClockingHistory_Read";
        [PermissionMeta(Name = "Tạo mới", Parents = new[] { FingerPrintLog_Read, Clocking_Update })]
        public const string ClockingHistory_Create = "ClockingHistory_Create";
        [PermissionMeta(Name = "Sửa", Parents = new[] { Branch_Create, Branch_Update, Clocking_Delete })]
        public const string ClockingHistory_Update = "ClockingHistory_Update";

        [PermissionMeta(Name = "Xem DS", Parents = new[] { PayslipPayment_Create, PayslipPayment_Update, EmployeeAdjustment_Read, CashFlow_Read, PayRate_Update, FingerPrintLog_Read, Commission_Update })]
        public const string Paysheet_Read = "Paysheet_Read";
        [PermissionMeta(Name = "Thêm mới")]
        public const string Paysheet_Create = "Paysheet_Create";
        [PermissionMeta(Name = "Chốt lương")]
        public const string Paysheet_Complete = "Paysheet_Complete";
        [PermissionMeta(Name = "Xóa")]
        public const string Paysheet_Delete = "Paysheet_Delete";
        [PermissionMeta(Name = "Cập nhật", Parents = new[] { Paysheet_Create, Paysheet_Complete, Paysheet_Delete, Clocking_Update, Clocking_Delete, PayslipPayment_Create, Branch_Update, PayRate_Update, FingerPrintLog_Read, Commission_Update })]
        public const string Paysheet_Update = "Paysheet_Update";
        [PermissionMeta(Name = "Xuất file")]
        public const string Paysheet_Export = "Paysheet_Export";

        [PermissionMeta(Name = "Xem DS", Parents = new[] { FingerPrintLog_Read, Clocking_Read, Clocking_Update, User_Read, Paysheet_Export, Paysheet_Read, CashFlow_Read })]
        public const string Employee_Read = "Employee_Read";
        [PermissionMeta(Name = "Tạo mới")]
        public const string Employee_Create = "Employee_Create";
        [PermissionMeta(Name = "Cập nhật", Parents = new[] { User_Create, User_Update, User_Delete }, Children = new[] { Employee_Create, Employee_Delete, Department_Update, JobTitle_Update })]
        public const string Employee_Update = "Employee_Update";
        [PermissionMeta(Name = "Xóa", Parents = new[] { Clocking_Delete })]
        public const string Employee_Delete = "Employee_Delete";

        [PermissionMeta(Name = "Xem DS", Parents = new[] { Employee_Read, Employee_Create, Employee_Update, Paysheet_Read, Clocking_Read, Clocking_Create, Clocking_Delete })]
        public const string Department_Read = "Department_Read";
        [PermissionMeta(Name = "Tạo mới", Parents = new[] { Employee_Create, Employee_Update })]
        public const string Department_Create = "Department_Create";
        [PermissionMeta(Name = "Cập nhật", Parents = new[] { Employee_Create, Employee_Update })]
        public const string Department_Update = "Department_Update";
        [PermissionMeta(Name = "Xóa", Parents = new[] { Employee_Create, Employee_Update })]
        public const string Department_Delete = "Department_Delete";

        [PermissionMeta(Name = "Xem DS", Parents = new[] { Employee_Read, Employee_Create, Employee_Update, Paysheet_Read })]
        public const string JobTitle_Read = "JobTitle_Read";
        [PermissionMeta(Name = "Tạo mới", Parents = new[] { Employee_Create, Employee_Update })]
        public const string JobTitle_Create = "JobTitle_Create";
        [PermissionMeta(Name = "Cập nhật", Parents = new[] { Employee_Create, Employee_Update })]
        public const string JobTitle_Update = "JobTitle_Update";
        [PermissionMeta(Name = "Xóa", Parents = new[] { Employee_Create, Employee_Update })]
        public const string JobTitle_Delete = "JobTitle_Delete";

        [PermissionMeta(Name = "Xem DS", Parents = new[] { Clocking_Read, Paysheet_Read, Paysheet_Create, Paysheet_Complete })]
        public const string Holiday_Read = "Holiday_Read";
        [PermissionMeta(Name = "Tạo mới")]
        public const string Holiday_Create = "Holiday_Create";
        [PermissionMeta(Name = "Cập nhật")]
        public const string Holiday_Update = "Holiday_Update";
        [PermissionMeta(Name = "Xóa")]
        public const string Holiday_Delete = "Holiday_Delete";

        [PermissionMeta(Name = "Xem DS", Parents = new[] { Branch_Read, Clocking_Create, Clocking_Update, Clocking_Read, Paysheet_Read, Paysheet_Create, Paysheet_Complete })]
        public const string BranchSetting_Read = "BranchSetting_Read";
        [PermissionMeta(Name = "Tạo mới", Parents = new[] { Branch_Create, Branch_Update })]
        public const string BranchSetting_Create = "BranchSetting_Create";
        [PermissionMeta(Name = "Cập nhật", Parents = new[] { Branch_Create, Branch_Update })]
        public const string BranchSetting_Update = "BranchSetting_Update";

        [PermissionMeta(Name = "Xóa", Parents = new[] { Employee_Create, Employee_Update })]
        public const string EmployeeProfilePicture_Delete = "EmployeeProfilePicture_Delete";
        [PermissionMeta(Name = "Tạo mới", Parents = new[] { Employee_Create, Employee_Update })]
        public const string EmployeeProfilePicture_Create = "EmployeeProfilePicture_Create";
        [PermissionMeta(Name = "Xem DS", Parents = new[] { Employee_Create, Employee_Update, Employee_Read })]
        public const string EmployeeProfilePicture_Read = "EmployeeProfilePicture_Read";

        [PermissionMeta(Name = "Tạo mới", Parents = new[] { PayRate_Update })]
        public const string PayRate_Create = "PayRate_Create";
        [PermissionMeta(Name = "Cập nhật")]
        public const string PayRate_Update = "PayRate_Update";
        [PermissionMeta(Name = "Xem DS", Parents = new[] { Paysheet_Read, Paysheet_Create, Paysheet_Update, PayRate_Update, PayRate_Create })]
        public const string PayRate_Read = "PayRate_Read";

        [PermissionMeta(Name = "Tạo mới", Parents = new[] { PayRate_Create })]
        public const string PayRateDetail_Create = "PayRateDetail_Create";
        [PermissionMeta(Name = "Cập nhật", Parents = new[] { PayRate_Update })]
        public const string PayRateDetail_Update = "PayRateDetail_Update";
        [PermissionMeta(Name = "Xem DS", Parents = new[] { Paysheet_Create, Paysheet_Update, PayRate_Update, PayRate_Create })]
        public const string PayRateDetail_Read = "PayRateDetail_Read";

        [PermissionMeta(Name = "Xem DS")]
        public const string Shift_Read = "Shift_Read";
        [PermissionMeta(Name = "Tạo mới")]
        public const string Shift_Create = "Shift_Create";
        [PermissionMeta(Name = "Cập nhật", Parents = new[] { Shift_Delete })]
        public const string Shift_Update = "Shift_Update";
        [PermissionMeta(Name = "Xóa")]
        public const string Shift_Delete = "Shift_Delete";

        [PermissionMeta(Name = "Xem DS", Parents = new[] { PayRate_Read, PayRate_Update, PayRate_Create, Paysheet_Export, Paysheet_Create, Paysheet_Update, Paysheet_Complete, Paysheet_Read, EmployeeAdjustment_Read })]
        public const string Allowance_Read = "Allowance_Read";
        [PermissionMeta(Name = "Tạo mới", Parents = new[] { PayRate_Create, PayRate_Update })]
        public const string Allowance_Create = "Allowance_Create";
        [PermissionMeta(Name = "Cập nhật", Parents = new[] { PayRate_Create, PayRate_Update })]
        public const string Allowance_Update = "Allowance_Update";

        [PermissionMeta(Name = "Xem DS", Parents = new[] { PayRate_Read, PayRate_Update, PayRate_Create, Paysheet_Export, Paysheet_Create, Paysheet_Update, Paysheet_Complete, Paysheet_Read, EmployeeAdjustment_Read })]
        public const string Deduction_Read = "Deduction_Read";
        [PermissionMeta(Name = "Tạo mới", Parents = new[] { PayRate_Create, PayRate_Update })]
        public const string Deduction_Create = "Deduction_Create";
        [PermissionMeta(Name = "Cập nhật", Parents = new[] { PayRate_Create, PayRate_Update })]
        public const string Deduction_Update = "Deduction_Update";

        [PermissionMeta(Name = "Xem DS", Parents = new[] { EmployeeAdjustment_Read })]
        public const string PayslipPayment_Read = "PayslipPayment_Read";
        [PermissionMeta(Name = "Thêm mới")]
        public const string PayslipPayment_Create = "PayslipPayment_Create";
        [PermissionMeta(Name = "Cập nhật")]
        public const string PayslipPayment_Update = "PayslipPayment_Update";
        [PermissionMeta(Name = "Xóa", Parents = new[] { Paysheet_Update, Paysheet_Complete, Paysheet_Delete })]
        public const string PayslipPayment_Delete = "PayslipPayment_Delete";

        [PermissionMeta(Name = "Thêm mới", Parents = new[] { Paysheet_Create, Paysheet_Update, Paysheet_Complete })]
        public const string Payslip_Create = "Payslip_Create";
        [PermissionMeta(Name = "Cập nhật", Parents = new[] { Paysheet_Create, Paysheet_Update, Paysheet_Complete, Paysheet_Delete })]
        public const string Payslip_Update = "Payslip_Update";
        [PermissionMeta(Name = "Xem DS", Parents = new[] { Paysheet_Read, Paysheet_Create, Paysheet_Update, Paysheet_Complete, Paysheet_Export, PayslipPayment_Create, PayslipPayment_Update, EmployeeAdjustment_Read, CashFlow_Read })]
        public const string Payslip_Read = "Payslip_Read";
        [PermissionMeta(Name = "Hủy", Parents = new[] { Paysheet_Create, Paysheet_Update, Paysheet_Delete })]
        public const string Payslip_Delete = "Payslip_Delete";

        [PermissionMeta(Name = "Thêm mới", Parents = new[] { Paysheet_Create, Paysheet_Update, Paysheet_Complete })]
        public const string PayslipClocking_Create = "PayslipClocking_Create";
        [PermissionMeta(Name = "Cập nhật", Parents = new[] { Paysheet_Create, Paysheet_Update, Paysheet_Complete })]
        public const string PayslipClocking_Update = "PayslipClocking_Update";
        [PermissionMeta(Name = "Xóa", Parents = new[] { Paysheet_Create, Paysheet_Update, Paysheet_Complete, Paysheet_Delete })]
        public const string PayslipClocking_Delete = "PayslipClocking_Delete";
        [PermissionMeta(Name = "Xem DS", Parents = new[] { Paysheet_Create, Paysheet_Update, Paysheet_Complete, Paysheet_Read, Paysheet_Export })]
        public const string PayslipClocking_Read = "PayslipClocking_Read";

        [PermissionMeta(Name = "Thêm mới", Parents = new[] { Paysheet_Create, Paysheet_Update, Paysheet_Complete })]
        public const string PayslipDetail_Create = "PayslipDetail_Create";
        [PermissionMeta(Name = "Cập nhật", Parents = new[] { Paysheet_Create, Paysheet_Update, Paysheet_Complete })]
        public const string PayslipDetail_Update = "PayslipDetail_Update";
        [PermissionMeta(Name = "Xóa", Parents = new[] { Paysheet_Create, Paysheet_Update, Paysheet_Complete, Paysheet_Delete })]
        public const string PayslipDetail_Delete = "PayslipDetail_Delete";
        [PermissionMeta(Name = "Xem DS", Parents = new[] { Paysheet_Create, Paysheet_Update, Paysheet_Complete, Paysheet_Read, Paysheet_Export, PayRate_Update })]
        public const string PayslipDetail_Read = "PayslipDetail_Read";

        [PermissionMeta(Name = "Xem DS")]
        public const string EmployeeAdjustment_Read = "EmployeeAdjustment_Read";

        [PermissionMeta(Name = "Xem Sổ Quỹ")]
        public const string CashFlow_Read = "CashFlow_Read";

        [PermissionMeta(Name = "Xem chi nhánh")]
        public const string Branch_Read = "Branch_Read";
        [PermissionMeta(Name = "Thêm mới chi nhánh")]
        public const string Branch_Create = "Branch_Create";
        [PermissionMeta(Name = "Cập nhật chi nhánh")]
        public const string Branch_Update = "Branch_Update";

        [PermissionMeta(Name = "Xem người dùng")]
        public const string User_Read = "User_Read";
        [PermissionMeta(Name = "Thêm mới người dùng")]
        public const string User_Create = "User_Create";
        [PermissionMeta(Name = "Cập nhật người dùng")]
        public const string User_Update = "User_Update";
        [PermissionMeta(Name = "Xóa người dùng")]
        public const string User_Delete = "User_Delete";

        [PermissionMeta(Name = "Đọc danh sách tài khoản chấm công", Parents = new[] { Employee_Read, Employee_Delete, FingerPrintLog_Read })]
        public const string FingerPrint_Read = "FingerPrint_Read";
        [PermissionMeta(Name = "Tạo mới tài khoản chấm công", Parents = new[] { FingerPrint_Update })]
        public const string FingerPrint_Create = "FingerPrint_Create";
        [PermissionMeta(Name = "Xóa tài khoản chấm công", Parents = new[] { FingerPrint_Update, Employee_Delete })]
        public const string FingerPrint_Delete = "FingerPrint_Delete";
        [PermissionMeta(Name = "Cập nhật tài khoản chấm công")]
        public const string FingerPrint_Update = "FingerPrint_Update";

        [PermissionMeta(Name = "Đọc danh sách máy công", Parents = new[] { FingerPrintLog_Read })]
        public const string FingerMachine_Read = "FingerMachine_Read";
        [PermissionMeta(Name = "Tạo mới máy công")]
        public const string FingerMachine_Create = "FingerMachine_Create";
        [PermissionMeta(Name = "Cập nhật máy công")]
        public const string FingerMachine_Update = "FingerMachine_Update";
        [PermissionMeta(Name = "Xóa máy công")]
        public const string FingerMachine_Delete = "FingerMachine_Delete";
        [PermissionMeta(Name = "Xem DS", Parents = new[] { Paysheet_Read, Paysheet_Create, Paysheet_Update, Paysheet_Complete, Paysheet_Export })]
        public const string Commission_Read = "Commission_Read";
        [PermissionMeta(Name = "Thêm mới")]
        public const string Commission_Create = "Commission_Create";
        [PermissionMeta(Name = "Cập nhật")]
        public const string Commission_Update = "Commission_Update";
        [PermissionMeta(Name = "Xóa")]
        public const string Commission_Delete = "Commission_Delete";
        [PermissionMeta(Name = "Xuất file")]
        public const string Commission_Export = "Commission_Export";
        [PermissionMeta(Name = "Import")]
        public const string Commission_Import = "Commission_Import";

        [PermissionMeta(Name = "Đọc dữ liệu chấm công")]
        public const string FingerPrintLog_Read = "FingerPrintLog_Read";
        [PermissionMeta(Name = "Xóa dữ liệu chấm công")]
        public const string FingerPrintLog_Delete = "FingerPrintLog_Delete";
        [PermissionMeta(Name = "Xem DS", Parents = new[] { Commission_Read, Commission_Export })]
        public const string CommissionDetail_Read = "CommissionDetail_Read";
        [PermissionMeta(Name = "Thêm mới", Parents = new[] { Commission_Create, Commission_Update })]
        public const string CommissionDetail_Create = "CommissionDetail_Create";
        [PermissionMeta(Name = "Cập nhật", Parents = new[] { Commission_Update })]
        public const string CommissionDetail_Update = "CommissionDetail_Update";
        [PermissionMeta(Name = "Xóa", Parents = new[] { Commission_Delete })]
        public const string CommissionDetail_Delete = "CommissionDetail_Delete";

        [PermissionMeta(Name = "Xem DS", Parents = new[] { Commission_Read, Commission_Update, Commission_Export })]
        public const string CommissionBranch_Read = "CommissionBranch_Read";
        [PermissionMeta(Name = "Thêm mới", Parents = new[] { Commission_Create, Commission_Update })]
        public const string CommissionBranch_Create = "CommissionBranch_Create";
        [PermissionMeta(Name = "Cập nhật", Parents = new[] { Commission_Update })]
        public const string CommissionBranch_Update = "CommissionBranch_Update";
        [PermissionMeta(Name = "Xóa", Parents = new[] { Commission_Delete, Commission_Update })]
        public const string CommissionBranch_Delete = "CommissionBranch_Delete";

        [PermissionMeta(Name = "Xem DS", Disable = true)]
        public const string EmployeeBranch_Read = "EmployeeBranch_Read";
        [PermissionMeta(Name = "Thêm mới", Parents = new[] { Employee_Create, Employee_Update })]
        public const string EmployeeBranch_Create = "EmployeeBranch_Create";
        [PermissionMeta(Name = "Cập nhật", Parents = new[] { Employee_Update })]
        public const string EmployeeBranch_Update = "EmployeeBranch_Update";
        [PermissionMeta(Name = "Xóa", Parents = new[] { Employee_Delete, Employee_Update })]
        public const string EmployeeBranch_Delete = "EmployeeBranch_Delete";
        [PermissionMeta(Name = "Không cho phép xem chấm công, tính lương của nhân viên khác")]
        public const string EmployeeLimit_Read = "EmployeeLimit_Read";

        [PermissionMeta(Name = "Xem DS")]
        public const string GeneralSettingClocking_Read = "GeneralSettingClocking_Read";
        [PermissionMeta(Name = "Thêm mới")]
        public const string GeneralSettingClocking_Create = "GeneralSettingClocking_Create";
        [PermissionMeta(Name = "Cập nhật")]
        public const string GeneralSettingClocking_Update = "GeneralSettingClocking_Update";
        [PermissionMeta(Name = "Xóa")]
        public const string GeneralSettingClocking_Delete = "GeneralSettingClocking_Delete";

        [PermissionMeta(Name = "Xem DS")]
        public const string GeneralSettingCommission_Read = "GeneralSettingCommission_Read";
        [PermissionMeta(Name = "Thêm mới")]
        public const string GeneralSettingCommission_Create = "GeneralSettingCommission_Create";
        [PermissionMeta(Name = "Cập nhật")]
        public const string GeneralSettingCommission_Update = "GeneralSettingCommission_Update";
        [PermissionMeta(Name = "Xóa")]
        public const string GeneralSettingCommission_Delete = "GeneralSettingCommission_Delete";

        [PermissionMeta(Name = "Xem DS")]
        public const string GeneralSettingTimesheet_Read = "GeneralSettingTimesheet_Read";
        [PermissionMeta(Name = "Thêm mới")]
        public const string GeneralSettingTimesheet_Create = "GeneralSettingTimesheet_Create";
        [PermissionMeta(Name = "Cập nhật")]
        public const string GeneralSettingTimesheet_Update = "GeneralSettingTimesheet_Update";
        [PermissionMeta(Name = "Xóa")]
        public const string GeneralSettingTimesheet_Delete = "GeneralSettingTimesheet_Delete";

        [PermissionMeta(Name = "Xem DS", Parents = new[] { PayRate_Read })]
        public const string GeneralSettingPayrateTemplate_Read = "GeneralSettingPayrateTemplate_Read";
        [PermissionMeta(Name = "Thêm mới")]
        public const string GeneralSettingPayrateTemplate_Create = "GeneralSettingPayrateTemplate_Create";
        [PermissionMeta(Name = "Cập nhật")]
        public const string GeneralSettingPayrateTemplate_Update = "GeneralSettingPayrateTemplate_Update";
        [PermissionMeta(Name = "Xóa")]
        public const string GeneralSettingPayrateTemplate_Delete = "GeneralSettingPayrateTemplate_Delete";

        [PermissionMeta(Name = "Xem DS", Parents = new[] { Clocking_Export })]
        public const string GeneralSettingHoliday_Read = "GeneralSettingHoliday_Read";
        [PermissionMeta(Name = "Thêm mới")]
        public const string GeneralSettingHoliday_Create = "GeneralSettingHoliday_Create";
        [PermissionMeta(Name = "Cập nhật")]
        public const string GeneralSettingHoliday_Update = "GeneralSettingHoliday_Update";
        [PermissionMeta(Name = "Xóa")]
        public const string GeneralSettingHoliday_Delete = "GeneralSettingHoliday_Delete";
    }
}
