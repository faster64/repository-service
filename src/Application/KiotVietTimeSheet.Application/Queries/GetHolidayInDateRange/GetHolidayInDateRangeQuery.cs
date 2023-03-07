using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetHolidayInDateRange
{
    [RequiredPermission(TimeSheetPermission.GeneralSettingHoliday_Read)]
    public class GetHolidayInDateRangeQuery : QueryBase<List<HolidayDto>>
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public GetHolidayInDateRangeQuery(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
