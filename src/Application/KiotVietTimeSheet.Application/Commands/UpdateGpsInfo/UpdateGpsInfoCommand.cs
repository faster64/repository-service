using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.UpdateGpsInfo
{
    [RequiredPermission(TimeSheetPermission.TimeSheet_Update)]
    public class UpdateGpsInfoCommand : BaseCommand<GpsInfoDto>
    {
        public GpsInfoDto GpsInfo { get; set; }
       
        public UpdateGpsInfoCommand(GpsInfoDto gpsInfo)
        {
            GpsInfo = gpsInfo;
          
        }
    }
}
