using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Queries.GetUnPaidPayslipByEmployeeId
{
    [RequiredPermission(TimeSheetPermission.Payslip_Read, TimeSheetPermission.Paysheet_Read)]
    public sealed class GetUnPaidPayslipByEmployeeIdQuery : QueryBase<object>
    {
        public long EmployeeId { get; set; }

        public GetUnPaidPayslipByEmployeeIdQuery(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
