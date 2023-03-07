using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace KiotVietTimeSheet.Application.Commands.CreateOrUpdateSetting
{
    [RequiredPermission(TimeSheetPermission.GeneralSettingTimesheet_Update)]
    public class CreateOrUpdateTimeSheetSettingCommand : CreateOrUpdateSettingCommand
    {
        public CreateOrUpdateTimeSheetSettingCommand(SettingsDto setting, byte settingType) : base(setting, settingType) { }
    }
}
