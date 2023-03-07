using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace KiotVietTimeSheet.Application.Commands.CreateOrUpdateSetting
{
    [RequiredPermission(TimeSheetPermission.GeneralSettingClocking_Update)]
    public class CreateOrUpdateClockingSettingCommand : CreateOrUpdateSettingCommand
    {
        public CreateOrUpdateClockingSettingCommand(SettingsDto setting, byte settingType) : base(setting, settingType) { }

    }
}
