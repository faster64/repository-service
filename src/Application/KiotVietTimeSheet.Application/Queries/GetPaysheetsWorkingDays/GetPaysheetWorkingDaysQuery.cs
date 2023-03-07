using System;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheetsWorkingDays
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Read)]
    public class GetPaysheetWorkingDaysQuery : QueryBase<object>
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public GetPaysheetWorkingDaysQuery(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
