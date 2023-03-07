using System;

namespace KiotVietTimeSheet.Application.Dto
{
    public class HolidayDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int TenantId { get; set; }
        public int Days { get; set; }
    }
}
