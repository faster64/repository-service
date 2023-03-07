using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class DeductionReadOnlyRepository : OrmLiteRepository<Deduction, long>, IDeductionReadOnlyRepository
    {
        public DeductionReadOnlyRepository(IDbConnectionFactory db, IAuthService authService) : base(db, authService) { }
        public async Task<PagingDataSource<DeductionDto>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false) => await LoadSelectDataSourceAsync<DeductionDto>(query, null, includeSoftDelete);

    }
}
