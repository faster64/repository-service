using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetHolidayForPaySheet
{
    [RequiredPermission(TimeSheetPermission.Holiday_Read)]
    public class GetHolidayForPaySheetQuery : QueryBase<List<HolidayDto>>
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public GetHolidayForPaySheetQuery(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }
    }
}
