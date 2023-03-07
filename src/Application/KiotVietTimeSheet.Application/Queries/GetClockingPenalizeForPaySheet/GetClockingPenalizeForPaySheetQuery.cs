using System;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Queries.GetClockingPenalizeForPaySheet
{
    [RequiredPermission(TimeSheetPermission.Clocking_Read)]
    public class GetClockingPenalizeForPaySheetQuery : QueryBase<List<ClockingPenalizeDto>>
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public List<long> EmployeeIds { get; set; }

        public GetClockingPenalizeForPaySheetQuery(DateTime from, DateTime to, List<long> employeeIds)
        {
            From = from;
            To = to;
            EmployeeIds = employeeIds;
        }
    }
}
