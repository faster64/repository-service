using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.UnAssignUserEmployee
{
    [RequiredPermission(TimeSheetPermission.Employee_Update)]
    public class UnAssignUserEmployeeCommand : BaseCommand
    {
        public long UserId { get; set; }
        public int BlockUnit { get; }
        public UnAssignUserEmployeeCommand(long userId, int blockUnit)
        {
            UserId = userId;
            BlockUnit = blockUnit;
        }
    }
}
