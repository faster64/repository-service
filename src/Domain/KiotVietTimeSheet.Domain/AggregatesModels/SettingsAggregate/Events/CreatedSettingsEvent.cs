using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Events
{
    public class CreatedSettingsEvent : DomainEvent
    {
        public Settings Settings { get; set; }

        public CreatedSettingsEvent(Settings settings)
        {
            Settings = settings;
        }
    }
}
