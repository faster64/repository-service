using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Domain.Common
{
    public class ConfirmClockingExtra
    {
        public Clocking Clocking { get; set; }
        public ClockingHistory ClockingHistory { get; set; }
        public ClockingHistoryType? CheckInDateType { get; set; }
        public ClockingHistoryType? CheckOutDateType { get; set; }
        public bool IsCheckIn { get; set; }
    }
}