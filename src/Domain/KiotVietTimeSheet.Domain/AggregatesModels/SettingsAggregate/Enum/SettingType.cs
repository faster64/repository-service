using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Enum
{
    public enum SettingType
    {
        [Description("Chấm công")]
        Clocking = 1,
        [Description("Tính lương")]
        TimeSheet = 2,
        [Description("Hoa hồng")]
        Commission = 3
    }
}
