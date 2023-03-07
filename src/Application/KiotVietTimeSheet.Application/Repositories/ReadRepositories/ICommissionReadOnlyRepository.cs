using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface ICommissionReadOnlyRepository : IBaseReadOnlyRepository<Commission, long>
    {
        Task<PagingDataSource<CommissionDto>> FiltersAsync(ISqlExpression query, string[] includes, bool includeSoftDelete = false);
        Task<List<Commission>> GetListForCurrentBranch(CommissionQueryFilter filter);
        Task<List<Commission>> GetAllCommission();
    }
}
