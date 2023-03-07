using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Utilities;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Caching.Repositories
{
    public class CachedEmployeeReadOnlyRepository : EmployeeReadOnlyRepository
    {
        private readonly ICacheClient _cacheClient;

        public CachedEmployeeReadOnlyRepository(
            IDbConnectionFactory db,
            IAuthService authService,
            ICacheClient cacheClient
        ) : base(db, authService)
        {
            _cacheClient = cacheClient;
        }

        public override async Task<Employee> FindByIdAsync(object id, bool reference = false, bool includeSoftDelete = false)
        {
            var guardDataAccessSpecification = await GuardDataAccess(new DefaultTrueSpec<Employee>(), includeSoftDelete);
            var cacheKey = $"{CacheKeys.GetEntityCacheKey(AuthService.Context.TenantCode, typeof(Employee).Name, (long)id)}_{reference}_{CacheHelpers.GetObjectHashValue(null, guardDataAccessSpecification)}";
            return await _cacheClient.GetAsync(cacheKey, (key) => base.FindByIdAsync(id, reference, includeSoftDelete));
        }

        public override async Task<PagingDataSource<Employee>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false, string[] include = null)
        {
            include = new[] { "EmployeeBranches", "ProfilePictures", "Department", "JobTitle" };
            var guardDataAccessSpecification = await GuardDataAccess(new DefaultTrueSpec<Employee>(), includeSoftDelete);
            var cacheKey = $"{CacheKeys.GetListEntityCacheKey(AuthService.Context.TenantCode, typeof(Employee).Name, $"{CacheHelpers.GetObjectHashValue(query, guardDataAccessSpecification)}")}";
            return await _cacheClient.GetAsync(cacheKey, (key) => base.FiltersAsync(query, includeSoftDelete, include));
        }
    }
}
