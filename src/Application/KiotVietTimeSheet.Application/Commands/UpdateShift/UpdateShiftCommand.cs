using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.UpdateShift
{
    [RequiredPermission(TimeSheetPermission.Shift_Update, TimeSheetPermission.Shift_Read)]
    public class UpdateShiftCommand : BaseCommand<ShiftDto>
    {
        public long Id { get; set; }
        public ShiftDto Shift { get; set; }
        public bool IsGeneralSetting { get; set; }

        public UpdateShiftCommand(long id, ShiftDto shiftt, bool isGeneralSetting)
        {
            Id = id;
            Shift = shiftt;
            IsGeneralSetting = isGeneralSetting;
        }
    }
}
