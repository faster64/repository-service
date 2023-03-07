using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IClockingHistoryReadOnlyRepository : IBaseReadOnlyRepository<ClockingHistory, long>
    {
        Task<PagingDataSource<ClockingHistoryDto>> FiltersAsync(ISqlExpression query);
    }
}
