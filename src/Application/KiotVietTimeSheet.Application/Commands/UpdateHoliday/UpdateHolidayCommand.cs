using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.UpdateHoliday
{
    [Auth.Common.RequiredPermission(TimeSheetPermission.GeneralSettingPayrateTemplate_Update)]
    public class UpdateHolidayCommand : BaseCommand<HolidayDto>
    {
        public HolidayDto Holiday { get; set; }
        public bool IsAddClocking { get; set; }
        public bool IsCancelClocking { get; set; }
        public bool IsShowPopupOverLapClocking { get; }
        public UpdateHolidayCommand(HolidayDto holiday, bool isAddClocking, bool isCancelClocking, bool isShowPopupOverLapClocking)
        {
            Holiday = holiday;
            IsAddClocking = isAddClocking;
            IsCancelClocking = isCancelClocking;
            IsShowPopupOverLapClocking = isShowPopupOverLapClocking;
        }
    }
}
