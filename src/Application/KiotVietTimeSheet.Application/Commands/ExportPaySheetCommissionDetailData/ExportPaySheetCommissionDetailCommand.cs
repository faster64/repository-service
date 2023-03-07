using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.ExportPaySheetCommissionDetailData
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Export)]
    public class ExportPaySheetCommissionDetailCommand : BaseCommand<PaysheetDto>
    {
        public long PaySheetId { get; set; }
        public int BranchId { get; set; }
        public long EmployeeId { get; set; }
        public long PayslipId { get; set; }

        public ExportPaySheetCommissionDetailCommand(long paySheetId, int branchId, long employeeId, long payslipId)
        {
            PaySheetId = paySheetId;
            BranchId = branchId;
            EmployeeId = employeeId;
            PayslipId = payslipId;
        }
    }
}
