using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetHolidayTotalDays
{
    [RequiredPermission(TimeSheetPermission.GeneralSettingHoliday_Read)]
    public class GetHolidayTotalDaysQuery : QueryBase<int>
    {
        public ISqlExpression Query { get; set; }

        public GetHolidayTotalDaysQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
