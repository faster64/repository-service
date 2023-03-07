using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface ICommissionBranchReadOnlyRepository : IBaseReadOnlyRepository<CommissionBranch, long>
    {
        Task<PagingDataSource<CommissionBranchDto>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false);
    }
}
