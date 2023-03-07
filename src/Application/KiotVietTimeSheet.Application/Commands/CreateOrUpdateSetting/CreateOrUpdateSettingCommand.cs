using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Enum;

namespace KiotVietTimeSheet.Application.Commands.CreateOrUpdateSetting
{
    public class CreateOrUpdateSettingCommand : BaseCommand<SettingsDto>
    {
        public SettingsDto Setting { get; set; }
        public byte SettingType { get; set; }
        public CreateOrUpdateSettingCommand(SettingsDto setting, byte settingType)
        {
            Setting = setting;
            SettingType = settingType;
        }
    }
}