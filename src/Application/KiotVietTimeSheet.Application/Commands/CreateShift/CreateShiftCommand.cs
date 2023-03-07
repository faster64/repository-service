using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateShift
{
    [RequiredPermission(TimeSheetPermission.Shift_Create, TimeSheetPermission.Shift_Read)]
    public class CreateShiftCommand : BaseCommand<ShiftDto>
    {
        public ShiftDto Shift { get; set; }
        public bool IsGeneralSetting { get; set; }

        public CreateShiftCommand(ShiftDto shiftDto, bool isGeneralSetting)
        {
            Shift = shiftDto;
            IsGeneralSetting = isGeneralSetting;
        }
    }
}
