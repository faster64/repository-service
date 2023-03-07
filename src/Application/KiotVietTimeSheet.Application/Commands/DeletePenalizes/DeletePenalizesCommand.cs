using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.DeletePenalizes
{
    [RequiredPermission(TimeSheetPermission.Clocking_Delete)]
    public class DeletePenalizesCommand : BaseCommand
    {
        public long Id { get; }
        public DeletePenalizesCommand(long id)
        {
            Id = id;
        }
    }
}
