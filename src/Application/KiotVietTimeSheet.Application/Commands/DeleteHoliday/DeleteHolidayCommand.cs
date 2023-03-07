using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.DeleteHoliday
{
    [RequiredPermission(TimeSheetPermission.GeneralSettingHoliday_Delete)]
    public class DeleteHolidayCommand : BaseCommand<HolidayDto>
    {
        public long Id { get; set; }
        public bool IsAddClocking { get; set; }
        public bool IsOverLapClocking { get; set; }

        public DeleteHolidayCommand(long id, bool isAddClocking, bool isOverLapClocking)
        {
            Id = id;
            IsAddClocking = isAddClocking;
            IsOverLapClocking = isOverLapClocking;
        }
    }
}
