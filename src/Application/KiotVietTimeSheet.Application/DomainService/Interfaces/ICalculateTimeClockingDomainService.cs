using System;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.DomainService.Interfaces
{
    public interface ICalculateTimeClockingDomainService
    {
        Task<int> GetTimeLate(int tenantId, DateTime clockingStartTime, DateTime? checkedInDate);
        Task<int> GetTimeEarly(int tenantId, DateTime clockingEndTime, DateTime? checkedOutDate);
        Task<int> GetOverTimeBeforeShiftWork(int tenantId, DateTime clockingStartTime, DateTime? checkedInDate);
        Task<int> GetOverTimeAfterShiftWork(int tenantId, DateTime clockingEndTime, DateTime? checkedOutDate);
    }
}
