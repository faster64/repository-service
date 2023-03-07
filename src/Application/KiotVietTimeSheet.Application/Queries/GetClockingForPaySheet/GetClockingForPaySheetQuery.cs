using System;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Queries.GetClockingForPaySheet
{
    [RequiredPermission(TimeSheetPermission.Clocking_Read)]
    public class GetClockingForPaySheetQuery : QueryBase<List<ClockingDto>>
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public List<long> EmployeeIds { get; set; }

        public GetClockingForPaySheetQuery(DateTime from, DateTime to, List<long> employeeIds)
        {
            From = from;
            To = to;
            EmployeeIds = employeeIds;
        }
    }
}
