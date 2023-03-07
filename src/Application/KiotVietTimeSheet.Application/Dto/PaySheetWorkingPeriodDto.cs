using System;

namespace KiotVietTimeSheet.Application.Dto
{
    public class PaySheetWorkingPeriodDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
