using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums
{
    public enum ClockingStatusesExtension
    {
        [Description("Đã hủy")]
        Void = 0,
        [Description("Chưa vào - Chưa ra")]
        Created = 1,
        [Description("Đã vào - Chưa ra")]
        CheckInNoCheckOut = 2,
        [Description("Đã vào - Đã ra")]
        CheckInCheckOut = 3,
        [Description("Chưa vào - Đã ra")]
        CheckOutNoCheckIn = 4,
        [Description("Nghỉ")]
        WorkOff = 5
    }
}
