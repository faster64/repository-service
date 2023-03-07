using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;

namespace KiotVietTimeSheet.Application.Dto
{
    public class SettingObjectDto : SettingsToObject
    {
        public SettingObjectDto() { }

        public bool IsAllowUseClockingGps { get; set; } = true;
    }
}
