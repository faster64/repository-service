using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateGpsInfo
{
    [RequiredPermission(TimeSheetPermission.GeneralSettingClocking_Read, TimeSheetPermission.GeneralSettingClocking_Create)]
    public class CreateGpsInfoCommand : BaseCommand<GpsInfoDto>
    {
        public GpsInfoDto GpsInfoDto { get; set;}
        public CreateGpsInfoCommand(GpsInfoDto gpsInfoDto)
        {
            GpsInfoDto = gpsInfoDto;
        }
    }
}
