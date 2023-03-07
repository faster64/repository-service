using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IAllowanceReadOnlyRepository : IBaseReadOnlyRepository<Allowance, long>
    {
        Task<PagingDataSource<AllowanceDto>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false);
    }
}
