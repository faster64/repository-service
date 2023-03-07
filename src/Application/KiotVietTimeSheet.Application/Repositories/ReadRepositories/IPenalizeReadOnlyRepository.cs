using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IPenalizeReadOnlyRepository : IBaseReadOnlyRepository<Penalize, long>
    {
        Task<PagingDataSource<PenalizeDto>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false);
    }
}
