using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.ExportEmployeePayslipData
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Export)]
    public class ExportEmployeePayslipCommand : BaseCommand<PayslipDto>
    {
        public long PaySheetId { get; set; }
        public int BranchId { get; set; }
        public long EmployeeId { get; set; }
        public long PayslipId { get; set; }

        public ExportEmployeePayslipCommand(long paySheetId, int branchId, long employeeId, long payslipId)
        {
            PaySheetId = paySheetId;
            BranchId = branchId;
            EmployeeId = employeeId;
            PayslipId = payslipId;
        }
    }
}
