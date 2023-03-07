using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.DeleteMultipleEmployee
{
    [RequiredPermission(TimeSheetPermission.Employee_Delete)]
    public class DeleteMultipleEmployeeCommand : BaseCommand
    {
        public long[] Ids { get; set; }
        public DeleteMultipleEmployeeCommand(long[] ids)
        {
            Ids = ids;
        }
    }
}
