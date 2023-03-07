using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.UpdateDeviceForEmployeeClockingGps
{
    [RequiredPermission(ClockingGpsPermission.Full)]
    public class UpdateDeviceForEmployeeClockingGpsCommand : BaseCommand<EmployeeDto>
    {
        public long EmployeeId { get; set; }
        public string VerifyCode { get; set; }
        public string Os { get; set; }
        public string OsVersion { get; set; }
        public string Type { get; set; }

        public UpdateDeviceForEmployeeClockingGpsCommand(
            long employeeId,
            string verifyCode,
            string os,
            string osVersion,
            string type)
        {
            EmployeeId = employeeId;
            VerifyCode = verifyCode;
            Os = os;
            OsVersion = osVersion;
            Type = type;
        }
    }
}