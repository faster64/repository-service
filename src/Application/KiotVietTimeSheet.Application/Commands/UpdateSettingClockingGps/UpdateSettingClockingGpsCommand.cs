using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.UpdateSettingClockingGps
{
    [RequiredPermission(TimeSheetPermission.GeneralSettingClocking_Update)]
    public class UpdateSettingClockingGpsCommand : BaseCommand<bool>
    {
        public bool UseClockingGps { get; set; }
        public UpdateSettingClockingGpsCommand(bool useClockingGps)
        {
            UseClockingGps = useClockingGps;
        }
    }
}