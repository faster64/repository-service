using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Queries.GetClockingsMultipleBranch
{
    public class GetClockingsMultipleBranchForCalendarQuery : QueryBase<PagingDataSource<ClockingDto>>
    {
        public List<int> BranchIds { get; set; }
        public List<byte> ClockingHistoryStates { get; set; }
        public List<long> DepartmentIds { get; set; }
        public List<long> ShiftIds { get; set; }
        public List<long> EmployeeIds { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<byte> ClockingStatusExtension { get; set; }

        public GetClockingsMultipleBranchForCalendarQuery(List<int> branchIds,
            List<byte> clockingHistoryStates, List<long> departmentIds, List<long> shiftIds, List<long> employeeIds,
            DateTime startTime, DateTime endTime, List<byte> clockingStatusExtension)
        {
            BranchIds = branchIds;
            ClockingHistoryStates = clockingHistoryStates;
            DepartmentIds = departmentIds;
            ShiftIds = shiftIds;
            EmployeeIds = employeeIds;
            StartTime = startTime;
            EndTime = endTime;
            ClockingStatusExtension = clockingStatusExtension;
        }
    }
}
