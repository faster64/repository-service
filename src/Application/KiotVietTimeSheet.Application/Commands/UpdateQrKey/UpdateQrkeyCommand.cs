using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.UpdateQrKey
{
   
    [RequiredPermission(TimeSheetPermission.TimeSheet_Update)]
    public class UpdateQrkeyCommand : BaseCommand<string>
    {
        public long GpsInfoId { get; set; }

        public UpdateQrkeyCommand(long gpsInfoId)
        {
            GpsInfoId = gpsInfoId;

        }
    }
}
