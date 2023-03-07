using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Application.DomainService
{
    public interface IRejectClockingsDomainService
    {
        /// <summary>
        /// Thực hiện hủy các chi tiết làm việc
        /// </summary>
        /// <param name="clockings"></param>
        /// <returns></returns>
        Task<bool> RejectClockingsAsync(List<Clocking> clockings);
    }
}
