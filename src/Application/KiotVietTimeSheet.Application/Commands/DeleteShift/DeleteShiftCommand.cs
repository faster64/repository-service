using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.DeleteShift
{
    [RequiredPermission(TimeSheetPermission.Shift_Delete, TimeSheetPermission.Shift_Read)]
    public class DeleteShiftCommand : BaseCommand<ShiftDto>
    {
        public long Id { get; set; }

        public DeleteShiftCommand(long id)
        {
            Id = id;
        }
    }
}
