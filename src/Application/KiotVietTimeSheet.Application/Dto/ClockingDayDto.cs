using System;

namespace KiotVietTimeSheet.Application.Dto
{
    public class ClockingDayDto
    {
        public string Id { get; set; }
        public string ResourceId { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public ClockingDto Clocking { get; set; }
    }
}
