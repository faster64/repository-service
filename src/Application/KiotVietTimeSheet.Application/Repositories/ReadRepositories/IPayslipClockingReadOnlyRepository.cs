using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IPayslipClockingReadOnlyRepository : IBaseReadOnlyRepository<PayslipClocking, long>
    {
        Task<PagingDataSource<PayslipClocking>> GetPayslipsClockingByPayslipIdAsync(PayslipClockingByPayslipIdFilter filter);

    }
}
