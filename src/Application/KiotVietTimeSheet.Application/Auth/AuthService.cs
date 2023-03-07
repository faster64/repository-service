using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.SharedKernel.Auth;
using KiotVietTimeSheet.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Service.Interfaces;

namespace KiotVietTimeSheet.Application.Auth
{
    public class AuthService : IAuthService
    {
        public ExecutionContext Context { get; }
        public bool WithDeleted { get; set; }
        private readonly IPermissionService _permissionService;

        public AuthService(ExecutionContext context,
            IPermissionService permissionDomainService
        )
        {
            Context = context;
            _permissionService = permissionDomainService;
        }

        public async Task<bool> HasPermissions(string[] permissions)
        {
            if (Context.User.IsAdmin) return true;
            // If entire list permission were not enable, we would not need check permission
            // Nếu tất cả các quyền không cần kiểm tra quyền sẽ không kiểm tra quyền
            if (permissions.All(p => AttributeHelpers.GetConstFieldAttributeValue<TimeSheetPermission, bool, PermissionMetaAttribute>(p, x => x.Disable)))
                return true;

            IEnumerable<string> userPermissions;

            if (Context.User.SessionClockingGps?.LoginByClockingGps == true)
            {
                userPermissions = Context.User.SessionClockingGps.Permissions;
            }
            else
            {
                userPermissions = await GetPermission(permissions);
            }
            
            return userPermissions.Any() && permissions.All(p => userPermissions.Contains(p));
        }

        public async Task<bool> HasAnyPermission(string[] permissions)
        {
            if (Context.User.IsAdmin) return true;
            // If any permission was not enable, we would not need check permission
            // Nếu 1 trong các quyền không cần kiểm tra quyền sẽ không kiểm tra quyền
            if (permissions.Any(p => AttributeHelpers.GetConstFieldAttributeValue<TimeSheetPermission, bool, PermissionMetaAttribute>(p, x => x.Disable)))
                return true;

            var userPermissions = await GetPermission(permissions);
            return userPermissions.Any();
        }

        public bool HasPermittedOnBranch(int branchId, IList<int> authorizedBranchIds)
        {
            return (authorizedBranchIds ?? new List<int>()).Any(b => b == branchId);
        }

        public async Task<IList<int>> GetAuthorizedBranchIds()
        {
            return await _permissionService.GetAuthorizedBranch(Context.TenantId, Context.TenantCode, Context.User.Id,
                Context.User.IsAdmin);
        }

        public async Task<bool> HasAnyPermissionMapWithBranchId(string[] permissions, int branchId)
        {
            if (Context.User.IsAdmin) return true;
            // If any permission was not enable, we would not need check permission
            // Nếu 1 trong các quyền không cần kiểm tra quyền sẽ không kiểm tra quyền
            var getPermission = await _permissionService.GetPermissionMap(Context.TenantId, Context.TenantCode, Context.User.Id, Context.BranchId);
            return permissions.Any(p => AttributeHelpers.GetConstFieldAttributeValue<TimeSheetPermission, bool, PermissionMetaAttribute>(p, x => x.Disable)) || getPermission.Any(g => g.Value.Contains(branchId) && permissions.Contains(g.Key));
        }

        #region Private method
        private async Task<IEnumerable<string>> GetPermission(string[] permissions)
        {
            var perPermissionMap = await _permissionService.GetPermissionMap(Context.TenantId, Context.TenantCode, Context.User.Id, Context.BranchId);
            var atLeastNecessaryPermissionMap = await _permissionService.GetAtLeastNecessaryPermission(Context.TenantId, Context.TenantCode, Context.User.Id, Context.BranchId);

            var permission = perPermissionMap.Where(g => g.Value.Contains(Context.BranchId)).Select(g => g.Key)
                .ToList();
            var results = permission.Where(p => permissions.Contains(p)).ToList();
            results.AddRange(atLeastNecessaryPermissionMap.Where(p => permissions.Contains(p.Key) && permission.Any(cp => p.Value.Contains(cp))).Select(p => p.Key).ToList());
            // Currently, parents and children have similar functionality
            // Hiện tại, các quyền cha và con có chức năng như nhau
            foreach (var perm in permissions)
            {
                var parents = AttributeHelpers.GetConstFieldAttributeValue<TimeSheetPermission, string[], PermissionMetaAttribute>(perm, x => x.Parents);
                if (parents != null && permission.Any(p => parents.Contains(p)))
                    results.Add(perm);

                var children = AttributeHelpers.GetConstFieldAttributeValue<TimeSheetPermission, string[], PermissionMetaAttribute>(perm, x => x.Children);
                if (children != null && permission.Any(p => children.Contains(p)))
                    results.Add(perm);
            }
            return results.Distinct();
        }
        #endregion

    }
}
