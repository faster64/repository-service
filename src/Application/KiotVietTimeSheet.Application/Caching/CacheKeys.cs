namespace KiotVietTimeSheet.Application.Caching
{
    public static class CacheKeys
    {
        private const string PrefixCachePath = "caches:";
        private const string PrefixCachePathKiotViet = "cache:";

        public static string GetEntityCacheKey(string tenantCode, string entityType, object entityId)
        {
            return $@"{GetEntityCachePrefix(tenantCode)}:{entityType}:object_{entityId}";
        }

        public static string GetListEntityCacheKey(string tenantCode, string entityType, string key)
        {
            return $@"{GetEntityCachePrefix(tenantCode)}:{entityType}:ls_{key}";
        }

        private static string GetEntityCachePrefix(string tenantCode)
        {
            return $@"{PrefixCachePath}{tenantCode}";
        }

        public static string GetPermissionMapCacheKey(int tenantId, long userId)
        {
            return $@"{PrefixCachePathKiotViet}users:perm_maps_{tenantId}_{userId}";
        }

        public static string GetAuthorizedBranchCacheKey(int tenantId, long userId)
        {
            return $@"{PrefixCachePathKiotViet}users:Permitted_branch_{tenantId}_{userId}";
        }

        public static string GetAtLeastNecessaryPermissionCacheKey(int tenantId, long userId)
        {
            return $@"{PrefixCachePathKiotViet}users:AtLeast_Necessary_{tenantId}_{userId}";
        }

        public static string GetTimeSheetPosParamCacheKey(int tenantId)
        {
            return $@"{PrefixCachePathKiotViet}timeSheetPosParam:{tenantId}";
        }
    }
}
