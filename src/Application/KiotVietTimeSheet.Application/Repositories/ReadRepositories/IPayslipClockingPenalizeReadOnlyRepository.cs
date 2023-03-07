using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IPayslipClockingPenalizeReadOnlyRepository : IBaseReadOnlyRepository<PayslipClockingPenalize, long>
    {
        Task<PagingDataSource<PayslipClockingPenalize>> GetPayslipsClockingPenalizeByPayslipIdAsync(PayslipClockingPenalizeByPayslipIdFilter filter);

    }
}
