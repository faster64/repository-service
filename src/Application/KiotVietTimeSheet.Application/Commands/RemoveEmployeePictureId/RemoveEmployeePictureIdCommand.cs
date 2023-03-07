using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.RemoveEmployeePictureId
{
    [RequiredPermission(TimeSheetPermission.Employee_Update)]
    public class RemoveEmployeePictureIdCommand : BaseCommand
    {
        public long Id { get; set; }
        public RemoveEmployeePictureIdCommand(long id)
        {
            Id = id;
        }
    }
}
