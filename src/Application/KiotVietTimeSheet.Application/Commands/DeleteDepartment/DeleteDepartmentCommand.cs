using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.DeleteDepartment
{
    [RequiredPermission(TimeSheetPermission.Department_Delete)]
    public class DeleteDepartmentCommand : BaseCommand
    {
        public long Id { get; set; }

        public DeleteDepartmentCommand(long id)
        {
            Id = id;
        }
    }
}
