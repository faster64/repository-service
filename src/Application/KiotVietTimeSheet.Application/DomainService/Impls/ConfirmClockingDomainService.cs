using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Utilities;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.DomainService.Impls
{
    public class ConfirmClockingDomainService : IConfirmClockingDomainService
    {
        public ConfirmClockingDomainService(
            )
        {
        }

        public string GetContent(ConfirmClocking confirmClocking)
        {
            var result = string.Empty;
            ConfirmClockingExtra extra;

            try
            {
                extra = JsonConvert.DeserializeObject<ConfirmClockingExtra>(confirmClocking.Extra);
            }
            catch
            {
                return string.Empty;
            }

            if (extra == null) return result;

            var formatDateTime = "HH:mm";

            if (extra.IsCheckIn) result += $"Chấm công vào {confirmClocking.CheckTime.ToString(formatDateTime)}";
            else result += $"Chấm công ra {confirmClocking.CheckTime.ToString(formatDateTime)}";

            return result;
        }

        public ConfirmClockingType GetConfirmClockingType(bool isWrongGps, bool isNewDevice, string geoCoordinate)
        {
            if (string.IsNullOrEmpty(geoCoordinate) && isNewDevice) return ConfirmClockingType.NewDeviceAndNotShareLocation;

            if (string.IsNullOrEmpty(geoCoordinate)) return ConfirmClockingType.NotShareLocation;

            if (isWrongGps && isNewDevice) return ConfirmClockingType.NewDeviceAndWrongGps;

            if (isNewDevice) return ConfirmClockingType.NewDevice;

            if (isWrongGps) return ConfirmClockingType.WrongGps;

            return ConfirmClockingType.Unknown;
        }

        public bool IsTypeNewDevice(byte confirmClockingType)
        {
            var ls = new List<byte> { (byte)ConfirmClockingType.NewDevice, (byte)ConfirmClockingType.NewDeviceAndWrongGps, (byte)ConfirmClockingType.NewDeviceAndNotShareLocation };

            if (ls.Any(item => item == confirmClockingType)) return true;

            return false;
        }
    }
}
