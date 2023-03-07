using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.WriteRepositories
{
    public interface IPaysheetWriteOnlyRepository : IBaseWriteOnlyRepository<Paysheet>
    {

        /// <summary>
        /// Thực hiện cập nhật thông tin của một bảng lương (không cập nhật các reference)
        /// </summary>
        /// <param name="paysheet"></param>
        /// <returns></returns>
        Task<Paysheet> UpdatePaysheetAsync(Paysheet paysheet);

        /// <summary>
        /// Thực hiện lưu lại một bảng lương (lưu tất cả các reference) vào Db
        /// </summary>
        /// <param name="paysheet"></param>
        /// <param name="isUpdate"></param>
        /// <returns></returns>
        Task<Paysheet> StoreAsync(Paysheet paysheet, bool isUpdate = false);

        /// <summary>
        /// Lấy danh sách bảng lương chứa một trong số các nhân viên
        /// </summary>
        /// <param name="employeeIds"></param>
        /// <returns></returns>
        Task<List<Paysheet>> GetPaysheetDraftAndTempByEmployeeIds(List<long> employeeIds);

        /// <summary>
        /// Lấy danh sách các bảng lương chứa một trong các chi tiết làm việc
        /// </summary>
        /// <param name="clockings"></param>
        /// <returns></returns>
        Task<List<Paysheet>> GetPaysheetDraftAndTempByClockings(List<Clocking> clockings);

        Task<List<Paysheet>> GetPaysheetDraftAndTempByCommissionIds(List<long> commissionIds);
    }
}
