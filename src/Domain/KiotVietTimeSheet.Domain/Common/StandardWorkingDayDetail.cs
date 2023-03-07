using System;

namespace KiotVietTimeSheet.Domain.Common
{
    public class StandardWorkingDayDetail
    {
        public DateTime Day { get; set; }
        public double StandardWorkingTime { get; set; }
        public int Late { get; set; }
        public int Early { get; set; }
    }
}
