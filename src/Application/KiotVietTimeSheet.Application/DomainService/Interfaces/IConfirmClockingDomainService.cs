using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.Utilities;

namespace KiotVietTimeSheet.Application.DomainService.Interfaces
{
    public interface IConfirmClockingDomainService
    {
        string GetContent(ConfirmClocking confirmClocking);
        ConfirmClockingType GetConfirmClockingType(bool isWrongGps, bool isNewDevice, string geoCoordinate);
        bool IsTypeNewDevice(byte confirmClockingType);
    }
}
