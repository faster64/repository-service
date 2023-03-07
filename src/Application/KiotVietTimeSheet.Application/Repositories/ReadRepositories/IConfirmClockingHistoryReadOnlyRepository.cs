using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IConfirmClockingHistoryReadOnlyRepository : IBaseReadOnlyRepository<ConfirmClockingHistory, long>
    {
        Task<PagingDataSource<ConfirmClockingHistory>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false, string[] include = null);
    }
}
