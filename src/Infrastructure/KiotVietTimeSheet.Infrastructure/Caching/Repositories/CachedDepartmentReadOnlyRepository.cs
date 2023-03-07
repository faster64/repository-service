using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Utilities;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Caching.Repositories
{
    public class CachedDepartmentReadOnlyRepository : DepartmentReadOnlyRepository
    {
        private readonly ICacheClient _cacheClient;

        public CachedDepartmentReadOnlyRepository(
            IDbConnectionFactory db,
            IAuthService authService,
            ICacheClient cacheClient
        ) : base(db, authService)
        {
            _cacheClient = cacheClient;
        }

        public override async Task<PagingDataSource<DepartmentDto>> FiltersAsync(ISqlExpression query)
        {
            var guardDataAccessSpecification = await GuardDataAccess(new DefaultTrueSpec<Department>());
            var cacheKey = $"{CacheKeys.GetListEntityCacheKey(AuthService.Context.TenantCode, typeof(Department).Name, $"{CacheHelpers.GetObjectHashValue(query, guardDataAccessSpecification)}")}";
            return await _cacheClient.GetAsync(cacheKey, (key) => base.FiltersAsync(query));
        }
    }
}
