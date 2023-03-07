using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.UpdateClockingForClockingGps
{
    [RequiredPermission(ClockingGpsPermission.Full)]
    public class UpdateClockingForClockingGpsCommand : BaseCommand<ClockingDto>
    {
        public ClockingDto ClockingDto { get; set; }
        public ClockingHistoryDto ClockingHistoryDto { get; set; }
        public string GeoCoordinate { get; set; }
        public string IdentityKeyClocking { get; set; }
        public bool AcceptWrongGps { get; set; }

        public UpdateClockingForClockingGpsCommand(ClockingDto clockingDto, ClockingHistoryDto clockingHistoryDto, string geoCoordinate, string identityKeyClocking, bool acceptWrongGps)
        {
            ClockingDto = clockingDto;
            ClockingHistoryDto = clockingHistoryDto;
            GeoCoordinate = geoCoordinate;
            IdentityKeyClocking = identityKeyClocking;
            AcceptWrongGps = acceptWrongGps;
        }
    }
}