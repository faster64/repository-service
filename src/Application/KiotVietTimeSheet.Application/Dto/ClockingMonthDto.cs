using System;

namespace KiotVietTimeSheet.Application.Dto
{
    public class ClockingMonthDto
    {
        public string Id { get; set; }
        public string ResourceId { get; set; }
        public long EmployeeId { get; set; }
        public long WorkById { get; set; }
        public long ShiftId { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public ClockingDto Clocking { get; set; }
    }
}
