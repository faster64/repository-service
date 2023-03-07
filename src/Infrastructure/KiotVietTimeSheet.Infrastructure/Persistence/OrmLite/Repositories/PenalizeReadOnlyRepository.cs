using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class PenalizeReadOnlyRepository : OrmLiteRepository<Penalize, long>, IPenalizeReadOnlyRepository
    {
        public PenalizeReadOnlyRepository(IDbConnectionFactory db, IAuthService authService) : base(db, authService) { }

        public async Task<PagingDataSource<PenalizeDto>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false) => await LoadSelectDataSourceAsync<PenalizeDto>(query, null, includeSoftDelete);
    }
}
