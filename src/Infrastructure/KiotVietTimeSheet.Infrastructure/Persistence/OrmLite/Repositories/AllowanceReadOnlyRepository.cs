using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class AllowanceReadOnlyRepository : OrmLiteRepository<Allowance, long>, IAllowanceReadOnlyRepository
    {
        public AllowanceReadOnlyRepository(IDbConnectionFactory db, IAuthService authService) : base(db, authService) { }
        public async Task<PagingDataSource<AllowanceDto>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false) => await LoadSelectDataSourceAsync<AllowanceDto>(query, null, includeSoftDelete);
    }
}
