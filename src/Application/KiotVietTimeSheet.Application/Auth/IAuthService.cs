using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.SharedKernel.Auth;

namespace KiotVietTimeSheet.Application.Auth
{
    public interface IAuthService
    {
        ExecutionContext Context { get; }

        bool WithDeleted { get; set; }

        Task<bool> HasPermissions(string[] permissions);

        bool HasPermittedOnBranch(int branchId, IList<int> authorizedBranchIds);

        Task<bool> HasAnyPermission(string[] permissions);

        Task<bool> HasAnyPermissionMapWithBranchId(string[] permissions, int branchId);

        Task<IList<int>> GetAuthorizedBranchIds();
    }
}
