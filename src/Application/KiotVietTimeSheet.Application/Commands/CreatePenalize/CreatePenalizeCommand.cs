using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreatePenalize
{
    [RequiredPermission(TimeSheetPermission.Clocking_Delete)]
    public class CreatePenalizeCommand : BaseCommand<PenalizeDto>
    {
        public PenalizeDto Penalize { get; }
        public CreatePenalizeCommand(PenalizeDto penalize)
        {
            Penalize = penalize;
        }
    }
}
