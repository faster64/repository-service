using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.DeleteMultipleEmployee
{
    [RequiredPermission(TimeSheetPermission.Employee_Delete)]
    public class DeleteMultipleEmployeeByBranchIdCommand : BaseCommand
    {
        public long BranchId { get; set; }
        public DeleteMultipleEmployeeByBranchIdCommand(long id)
        {
            BranchId = id;
        }
    }
}
