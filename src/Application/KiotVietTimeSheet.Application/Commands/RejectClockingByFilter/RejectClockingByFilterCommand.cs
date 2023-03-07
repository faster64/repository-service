using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.RejectClockingByFilter
{
    [RequiredPermission(TimeSheetPermission.Clocking_Delete)]
    public class RejectClockingByFilterCommand : BaseCommand<List<ClockingDto>>
    {
        public long BranchId { get; set; }
        public List<long> EmployeeIds { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long ShiftId { get; set; }
        public List<byte> StatusesExtension { get; set; }

        public RejectClockingByFilterCommand(long branchId, List<long> employeeIds, DateTime startTime, DateTime endTime, long shiftId, List<byte> statusesExtension)
        {
            BranchId = branchId;
            EmployeeIds = employeeIds;
            StartTime = startTime;
            EndTime = endTime;
            ShiftId = shiftId;
            StatusesExtension = statusesExtension;
        }
    }
}
