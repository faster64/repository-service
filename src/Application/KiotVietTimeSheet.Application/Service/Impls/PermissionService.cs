using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.Service.Interfaces;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Domain.Common;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Service.Impls
{
    public class PermissionService : IPermissionService
    {
        private Dictionary<string, IList<int>> _permissionMap;
        private IDictionary<string, IList<string>> _atLeastNecessaryPermission;
        private List<int> _authorizedBranch;
        private readonly IKiotVietInternalService _kiotVietInternalService;
        private readonly ICacheClient _cacheClient;
        private const int CacheTimeNumber = 1;

        public PermissionService(
            IKiotVietInternalService kiotVietInternalService,
            ICacheClient cacheClient
            )
        {
            _kiotVietInternalService = kiotVietInternalService;
            _cacheClient = cacheClient;
        }

        public async Task<Dictionary<string, IList<int>>> GetPermissionMap(int tenantId, string tenantCode, long userId,
            int branchId)
        {
            try
            {
                using (var tokenSource = new CancellationTokenSource(Constant.MillisecondsDelay))
                {
                    if (_permissionMap != null) return _permissionMap;

                    var permissionKey = CacheKeys.GetPermissionMapCacheKey(tenantId, userId);
                    var permissionMap = _cacheClient.GetOrDefault<Dictionary<string, IList<int>>>(permissionKey);

                    if (permissionMap != null) return permissionMap;

                    var permissions = await _kiotVietInternalService.GetPermissionByCurrentUser(tenantId, tenantCode,
                        userId,
                        branchId, tokenSource.Token);
                    if (permissions != null)
                    {
                        _cacheClient.Set(permissionKey, permissions.PermissionMap, TimeSpan.FromHours(CacheTimeNumber));
                        _permissionMap = permissions.PermissionMap;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                throw HttpError.BadRequest(Constant.TaskCanceledExceptionMessage);
            }
            catch (Exception ex)
            {
                throw HttpError.Unauthorized(ex.Message);
            }
            return _permissionMap;
        }

        public async Task<IDictionary<string, IList<string>>> GetAtLeastNecessaryPermission(int tenantId,
            string tenantCode, long userId, int branchId)
        {
            try
            {
                using (var tokenSource = new CancellationTokenSource(Constant.MillisecondsDelay))
                {

                    if (_atLeastNecessaryPermission != null) return _atLeastNecessaryPermission;

                    var atLeastNecessaryPermissionKey = CacheKeys.GetAtLeastNecessaryPermissionCacheKey(tenantId, userId);
                    var atLeastNecessaryPermissionCache = _cacheClient.GetOrDefault<Dictionary<string, IList<string>>>(atLeastNecessaryPermissionKey);

                    if (atLeastNecessaryPermissionCache != null) return atLeastNecessaryPermissionCache;

                    var atLeastNecessaryPermission =
                        await _kiotVietInternalService.GetAtLeastNecessaryPermission(tenantId, tenantCode, userId,
                            branchId, tokenSource.Token);
                    if (atLeastNecessaryPermission != null)
                    {
                        _cacheClient.Set(atLeastNecessaryPermissionKey, atLeastNecessaryPermission, TimeSpan.FromHours(CacheTimeNumber));
                        _atLeastNecessaryPermission = atLeastNecessaryPermission;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                throw HttpError.BadRequest(Constant.TaskCanceledExceptionMessage);
            }
            catch (Exception ex)
            {
                throw HttpError.Unauthorized(ex.Message);
            }

            return _atLeastNecessaryPermission;
        }

        public async Task<List<int>> GetAuthorizedBranch(int tenantId,
            string tenantCode, long userId, bool isAdmin)
        {
            try
            {
                using (var tokenSource = new CancellationTokenSource(Constant.MillisecondsDelay))
                {
                    var authorizedBranchKey = CacheKeys.GetAuthorizedBranchCacheKey(tenantId, userId);

                    var authorizedBranchCache = _cacheClient.GetOrDefault<List<int>>(authorizedBranchKey);

                    if (authorizedBranchCache != null) return authorizedBranchCache;

                    var authorizedBranchList = await _kiotVietInternalService.GetAuthorizedBranch(tenantId, tenantCode,
                        userId, isAdmin, tokenSource.Token);
                    if (authorizedBranchList != null)
                    {
                        _cacheClient.Set(authorizedBranchKey, authorizedBranchList, TimeSpan.FromHours(CacheTimeNumber));
                        _authorizedBranch = authorizedBranchList;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                throw HttpError.BadRequest(Constant.TaskCanceledExceptionMessage);
            }
            catch (Exception ex)
            {
                throw HttpError.Unauthorized(ex.Message);
            }
            return _authorizedBranch;
        }
    }
}
