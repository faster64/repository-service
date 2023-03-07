using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class CommissionBranchReadOnlyRepository : OrmLiteRepository<CommissionBranch, long>, ICommissionBranchReadOnlyRepository
    {
        public CommissionBranchReadOnlyRepository(IDbConnectionFactory db, IAuthService authService) : base(db, authService) { }
        public async Task<PagingDataSource<CommissionBranchDto>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false) => await LoadSelectDataSourceAsync<CommissionBranchDto>(query, null, includeSoftDelete);

    }
}
