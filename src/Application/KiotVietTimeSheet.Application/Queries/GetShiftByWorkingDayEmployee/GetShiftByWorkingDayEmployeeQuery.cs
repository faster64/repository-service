using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetShiftByWorkingDayEmployee
{
    public sealed class GetShiftByWorkingDayEmployeeQuery : QueryBase<List<ShiftDto>>
    {
        public long EmployeeId { get; set; }
        public DateTime StartTime { get; set; }

        public GetShiftByWorkingDayEmployeeQuery(long employeeId, DateTime startTime)
        {
            EmployeeId = employeeId;
            StartTime = startTime;
        }
    }
}
