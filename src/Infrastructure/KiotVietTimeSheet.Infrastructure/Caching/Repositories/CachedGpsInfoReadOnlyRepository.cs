using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.Data;

namespace KiotVietTimeSheet.Infrastructure.Caching.Repositories
{
    public class CachedGpsInfoReadOnlyRepository : GpsInfoReadOnlyRepository
    {
        private readonly ICacheClient _cacheClient;

        public CachedGpsInfoReadOnlyRepository(
            IDbConnectionFactory db,
            IAuthService authService,
            ICacheClient cacheClient
        ) : base(db, authService)
        {
            _cacheClient = cacheClient;
        }

        public override async Task<GpsInfo> GetForClockingGps(TenantModel tenant, string qrKey)
        {
            var gpsInfos = await GetListForClockingGps(tenant);
            var result = gpsInfos.FirstOrDefault(x => x.QrKey == qrKey);
            return result;
        }

        public override async Task<List<GpsInfo>> GetListForClockingGps(TenantModel tenant)
        {
            var cacheKey = $"{CacheKeys.GetListEntityCacheKey(tenant.Code, typeof(GpsInfo).Name, "clockinggps")}";
            var gpsInfos = await _cacheClient.GetAsync(cacheKey, (key) =>
            {
                return base.GetListForClockingGps(tenant);
            });

            return gpsInfos;
        }
    }
}
