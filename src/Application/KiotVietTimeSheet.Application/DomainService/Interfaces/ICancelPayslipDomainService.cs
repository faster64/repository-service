using System.Collections.Generic;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.DomainService.Interfaces
{
    public interface ICancelPayslipDomainService
    {
        /// <summary>
        /// Hủy phiếu lương
        /// </summary>
        /// <param name="ids">Danh sách id phiếu lương muốn hủy</param>
        /// <param name="withoutEvent">nếu true, thì sẽ không fire event hủy payslip</param>
        /// <returns></returns>
        Task CancelAsync(List<long> ids, bool withoutEvent = false);
    }
}
