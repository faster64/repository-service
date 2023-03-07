using System;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.ServiceClients.RequestModels
{
    public class MakeClockingCalendarReq
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<byte> ClockingStatusExtension { get; set; }
        public List<byte> ClockingHistoryStates { get; set; }
        public List<long> ShiftIds { get; set; }
        public List<long> EmployeeIds { get; set; }
        public List<long> DepartmentIds { get; set; }
        public List<int> BranchIds { get; set; }
    }
}
