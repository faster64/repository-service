using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Queries.CheckEmployeeAssignUserId
{
    [RequiredPermission(TimeSheetPermission.Employee_Update)]
    public class CheckEmployeeAssignUserIdQuery : QueryBase<bool>
    {
        public long EmployeeId { get; set; }
        public long UserId { get; set; }
        public bool IsCreateUser { get; set; }
        public CheckEmployeeAssignUserIdQuery(long employeeId, long userId, bool isCreateUser)
        {
            EmployeeId = employeeId;
            UserId = userId;
            IsCreateUser = isCreateUser;
        }
    }
}
