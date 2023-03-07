using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetHolidayById
{
    [RequiredPermission(TimeSheetPermission.GeneralSettingHoliday_Read)]
    public class GetHolidayByIdQuery : QueryBase<HolidayDto>
    {
        public long Id { get; set; }

        public GetHolidayByIdQuery(long id)
        {
            Id = id;
        }
    }
}
