using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.UpdatePenalize
{
    [RequiredPermission(TimeSheetPermission.Clocking_Update)]
    public class UpdatePenalizeCommand : BaseCommand<PenalizeDto>
    {
        public PenalizeDto Penalize { get; }
        public UpdatePenalizeCommand(PenalizeDto penalize)
        {
            Penalize = penalize;
        }
    }
}
