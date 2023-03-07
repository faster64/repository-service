using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums
{
    public enum TimeKeepingTypes
    {
        [Description("Thủ công")]
        Manual = 1,
        [Description("Chấm công vân tay")]
        Fingerprint = 2,
        [Description("Điều chỉnh")]
        Updated = 3,
        [Description("Chấm công tự động")]
        Automation = 4,
        [Description("Chấm công gps")]
        Gps = 5,
        [Description("Hệ thống tự động chấm công")]
        AutomationBySetting = 6
    }
}
