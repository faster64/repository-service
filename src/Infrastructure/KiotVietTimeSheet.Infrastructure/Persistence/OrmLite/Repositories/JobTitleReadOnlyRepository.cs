using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using ServiceStack.Data;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class JobTitleReadOnlyRepository : OrmLiteRepository<JobTitle, long>, IJobTitleReadOnlyRepository
    {
        public JobTitleReadOnlyRepository(IDbConnectionFactory db, IAuthService authService)
               : base(db, authService)
        {

        }

        public virtual async Task<PagingDataSource<JobTitleDto>> FiltersAsync(ISqlExpression query)
        {
            return await LoadSelectDataSourceAsync<JobTitleDto>(query);
        }
    }
}
