using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using KiotVietTimeSheet.Utilities;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using Shift = KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models.Shift;

namespace KiotVietTimeSheet.Infrastructure.Caching.Repositories
{
    public class CachedShiftReadOnlyRepository : ShiftReadOnlyRepository
    {
        private readonly ICacheClient _cacheClient;

        public CachedShiftReadOnlyRepository(
            IDbConnectionFactory db,
            IAuthService authService,
            ICacheClient cacheClient
        ) : base(db, authService)
        {
            _cacheClient = cacheClient;
        }

        public override async Task<Shift> FindByIdAsync(object id, bool reference = false, bool includeSoftDelete = false)
        {
            var cacheKey = $"{CacheKeys.GetEntityCacheKey(AuthService.Context.TenantCode, typeof(Shift).Name, (long)id)}_{reference}_{includeSoftDelete}";
            return await _cacheClient.GetAsync(cacheKey, (key) => base.FindByIdAsync(id, reference, includeSoftDelete));
        }

        public override async Task<PagingDataSource<ShiftDto>> FiltersAsync(ISqlExpression query)
        {
            var guardDataAccessSpecification = await GuardDataAccess(new DefaultTrueSpec<Shift>());
            var cacheKey = $"{CacheKeys.GetListEntityCacheKey(AuthService.Context.TenantCode, typeof(Shift).Name, $"{CacheHelpers.GetObjectHashValue(query, guardDataAccessSpecification)}")}";
            return await _cacheClient.GetAsync(cacheKey, (key) => base.FiltersAsync(query));
        }

        public override async Task<List<Shift>> GetBySpecificationAsync(ISpecification<Shift> spec,
            bool reference = false, bool includeSoftDelete = false)
        {
            var guardDataAccessSpecification = await GuardDataAccess(new DefaultTrueSpec<Shift>(), includeSoftDelete);
            var cacheKey =
                $"{CacheKeys.GetListEntityCacheKey(AuthService.Context.TenantCode, typeof(Shift).Name, $"{CacheHelpers.GetObjectHashValue(spec, guardDataAccessSpecification)}")}_{reference}";
            return await _cacheClient.GetAsync(cacheKey, (key) => base.GetBySpecificationAsync(spec, reference, includeSoftDelete));
        }
    }
}
