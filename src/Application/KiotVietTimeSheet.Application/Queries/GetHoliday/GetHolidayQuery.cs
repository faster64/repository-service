using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetHoliday
{
    [RequiredPermission(TimeSheetPermission.GeneralSettingHoliday_Read)]
    public class GetHolidayQuery : QueryBase<HolidayPagingDataSource>
    {
        public ISqlExpression Query { get; set; }

        public GetHolidayQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
