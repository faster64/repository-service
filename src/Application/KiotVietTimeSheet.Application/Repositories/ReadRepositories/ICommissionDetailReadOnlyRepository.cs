using System.Threading.Tasks;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface ICommissionDetailReadOnlyRepository : IBaseReadOnlyRepository<CommissionDetail, long>
    {
        Task<PagingDataSource<CommissionDetail>> GetListByQueryFilterAsync(CommissionDetailQueryFilter filter);
        Task<PagingDataSource<CommissionDetailDto>> GetCommissionDetailByProductId(ISqlExpression query);
        Task<PagingDataSource<CommissionDetailDto>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false);
    }
}
