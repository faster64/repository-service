using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Queries.GetTimesheetConfigForMobile
{
    [RequiredPermission(TimeSheetPermission.GeneralSettingTimesheet_Read)]
    public sealed class GetTimesheetConfigForMobileQuery : QueryBase<object>
    {

        public GetTimesheetConfigForMobileQuery()
        {

        }
    }
}
