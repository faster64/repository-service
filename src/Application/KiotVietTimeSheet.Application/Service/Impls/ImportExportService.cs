using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Service.Interfaces;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Service.Impls
{
    public class ImportExportService : IImportExportService
    {
        private readonly IAuthService _authService;
        private readonly ICacheClient _cacheClient;
        private readonly IPermissionService _permissionService;

        public ImportExportService(IAuthService authService,
            ICacheClient cacheClient,
            IPermissionService permissionService)
        {
            _authService = authService;
            _cacheClient = cacheClient;
            _permissionService = permissionService;
        }

        public async Task<ImportExportExecutionContext> GetExecutionContext()
        {
            var result = _authService.Context.ConvertTo<ImportExportExecutionContext>();
            result.RetailerId = _authService.Context.TenantId;
            result.RetailerCode = _authService.Context.TenantCode;
            result.User = _authService.Context.User;
            result.BranchId = _authService.Context.BranchId;
            result.GroupId = _authService.Context.User.GroupId;
            result.IndustryId = _authService.Context.User.IndustryId;
            result.Group = GetGroup(result.RetailerCode);

            return result;
        }

        public async Task<ImportExportSession> GetSession(object kvSession)
        {
            var importExportSession = kvSession.ConvertTo<ImportExportSession>();
            if (!importExportSession.CurrentUser.IsAdmin && importExportSession.PermissionMap == null)
            {
                importExportSession.PermissionMap = await _permissionService.GetPermissionMap(
                    importExportSession.CurrentRetailerId,
                    importExportSession.CurrentRetailerCode,
                    importExportSession.CurrentUser.Id,
                    importExportSession.CurrentBranchId);
            }

            return importExportSession;
        }

        private ImportExportKvGroup GetGroup(string retailerCode)
        {
            var keyObject = $@"{CacheKeys.GetEntityCacheKey(
                retailerCode,
                nameof(KiotVietTimeSheet),
                "KvGroup"
            )}*";
            var kvGroup = _cacheClient.GetOrDefault<ImportExportKvGroup>(keyObject);

            return kvGroup;
        }
    }
}
