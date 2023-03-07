using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Application.Caching;

namespace KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Common
{
    public class Helper
    {
        public void FlushCacheStore(ICacheClient cacheClient, string tenantCode, string entityName)
        {
            var cleanCacheKeys = new List<string>
                    {
                        $@"{CacheKeys.GetEntityCacheKey(tenantCode, entityName, "*")}",
                        CacheKeys.GetListEntityCacheKey(tenantCode, entityName, "*")
                    };
            if (cleanCacheKeys.Any())
                cacheClient.RemoveByParttern(cleanCacheKeys.ToArray());
        }

    }
}
