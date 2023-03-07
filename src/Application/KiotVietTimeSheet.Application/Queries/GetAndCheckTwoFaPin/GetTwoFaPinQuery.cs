using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Queries.GetAndCheckTwoFaPin
{
    [RequiredPermission(TimeSheetPermission.Employee_Read)]
    public class GetTwoFaPinQuery : QueryBase<string>
    {
        public long EmployeeId { get; set; }
        public long? UserId { get; set; }

        public GetTwoFaPinQuery(long employeeId, long? userId)
        {
            EmployeeId = employeeId;
            UserId = userId;
        }
    }
}
