using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetClockingForSwap
{
    [RequiredPermission(TimeSheetPermission.Clocking_Read)]
    public class GetClockingForSwapQuery : QueryBase<List<ClockingDto>>
    {
        public long EmployeeId { get; set; }
        public DateTime Day { get; set; }
        public long BranchId { get; set; }
        public long ShiftId { get; set; }

        public GetClockingForSwapQuery(long employeeId, DateTime day, long branchId, long shiftId)
        {
            EmployeeId = employeeId;
            Day = day;
            BranchId = branchId;
            ShiftId = shiftId;
        }
    }
}
