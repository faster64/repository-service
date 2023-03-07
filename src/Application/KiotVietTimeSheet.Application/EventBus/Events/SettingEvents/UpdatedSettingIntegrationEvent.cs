using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.SettingEvents
{
    public class UpdatedSettingIntegrationEvent : IntegrationEvent
    {
        public SettingsToObject OldSetting { get; set; }
        public SettingsDto NewSetting { get; set; }
        public byte SettingType { get; set; }

        public UpdatedSettingIntegrationEvent(SettingsToObject oldSetting, SettingsDto newSetting, byte settingType)
        {
            OldSetting = oldSetting;
            NewSetting = newSetting;
            SettingType = settingType;
        }

        [JsonConstructor]
        public UpdatedSettingIntegrationEvent()
        {

        }
    }
}