using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateHoliday
{
    [RequiredPermission(TimeSheetPermission.GeneralSettingHoliday_Create)]
    public class CreateHolidayCommand : BaseCommand<HolidayDto>
    {
        public HolidayDto HolidayDto { get; set; }
        public bool IsCancelClocking { get; set; }
        public bool IsShowPopupOverLapClocking { get; }
        public CreateHolidayCommand(HolidayDto holidayDto, bool isCancelClocking, bool isShowPopupOverLapClocking)
        {
            HolidayDto = holidayDto;
            IsCancelClocking = isCancelClocking;
            IsShowPopupOverLapClocking = isShowPopupOverLapClocking;
        }
    }
}
