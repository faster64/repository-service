using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;


namespace KiotVietTimeSheet.Application.Commands.RejectClockingByBranches
{
    [RequiredPermission(TimeSheetPermission.Clocking_Delete)]
    public class RejectClockingByBranchesCommand : BaseCommand<List<ClockingDto>>
    {
        public List<long> BranchIds { get; set; }
        public long EmployeeId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public byte Statuses { get; set; }
        public bool IforAllClockings { get; set; }

        public RejectClockingByBranchesCommand(List<long> branchIds, long employeeId, DateTime startTime, DateTime endTime, byte statuses, bool iforAllClockings)
        {
            BranchIds = branchIds;
            EmployeeId = employeeId;
            StartTime = startTime;
            EndTime = endTime;
            Statuses = statuses;
            IforAllClockings = iforAllClockings;
        }
    }
}