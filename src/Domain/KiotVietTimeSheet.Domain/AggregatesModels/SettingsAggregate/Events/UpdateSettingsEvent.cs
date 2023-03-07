using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Events
{
    public class UpdateSettingsEvent : DomainEvent
    {
        public Settings Settings { get; set; }
        public Settings OldSettings { get; set; }

        public UpdateSettingsEvent(Settings oldSettings, Settings settings)
        {
            Settings = settings;
            OldSettings = oldSettings;
        }
    }
}
