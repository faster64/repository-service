using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Utilities;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Caching.Repositories
{
    public class CachedHolidayReadOnlyRepository : HolidayReadOnlyRepository
    {
        #region Properties
        private readonly ICacheClient _cacheClient;
        #endregion

        public CachedHolidayReadOnlyRepository(
            IDbConnectionFactory db,
            IAuthService authService,
            ICacheClient cacheClient
            ) : base(db, authService)
        {
            _cacheClient = cacheClient;
        }

        public override async Task<Holiday> FindByIdAsync(object id, bool reference = false, bool includeSoftDelete = false)
        {
            var guardDataAccessSpecification = await GuardDataAccess(new DefaultTrueSpec<Holiday>(), includeSoftDelete);
            var cacheKey =
                $"{CacheKeys.GetEntityCacheKey(AuthService.Context.TenantCode, typeof(Holiday).Name, (long)id)}_{reference}_{CacheHelpers.GetObjectHashValue(null, guardDataAccessSpecification)}";
            return await _cacheClient.GetAsync(cacheKey, (key) => base.FindByIdAsync(id, reference, includeSoftDelete));
        }

        public override async Task<PagingDataSource<HolidayDto>> FiltersAsync(ISqlExpression query)
        {
            var guardDataAccessSpecification = await GuardDataAccess(new DefaultTrueSpec<Holiday>());
            var cacheKey =
                $"{CacheKeys.GetListEntityCacheKey(AuthService.Context.TenantCode, typeof(Holiday).Name, $"{CacheHelpers.GetObjectHashValue(query, guardDataAccessSpecification)}")}";
            return await _cacheClient.GetAsync(cacheKey, (key) => base.FiltersAsync(query));
        }
    }
}
