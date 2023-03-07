using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using ServiceStack.Data;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class DepartmentReadOnlyRepository : OrmLiteRepository<Department, long>, IDepartmentReadOnlyRepository
    {
        public DepartmentReadOnlyRepository(IDbConnectionFactory db, IAuthService authService)
            : base(db, authService)
        {

        }

        public virtual async Task<PagingDataSource<DepartmentDto>> FiltersAsync(ISqlExpression query)
        {
            return await LoadSelectDataSourceAsync<DepartmentDto>(query);
        }
    }
}
