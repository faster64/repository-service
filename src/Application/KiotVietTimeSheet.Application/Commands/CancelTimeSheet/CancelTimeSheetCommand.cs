using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CancelTimeSheet
{
    [RequiredPermission(TimeSheetPermission.Clocking_Delete)]
    public class CancelTimeSheetCommand : BaseCommand<Unit>
    {
        public long Id { get; set; }

        public CancelTimeSheetCommand(long id)
        {
            Id = id;
        }
    }
}
