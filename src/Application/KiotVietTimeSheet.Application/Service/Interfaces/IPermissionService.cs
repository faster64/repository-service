using System.Collections.Generic;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Service.Interfaces
{
    public interface IPermissionService
    {
        Task<Dictionary<string, IList<int>>> GetPermissionMap(int tenantId, string tenantCode, long userId,
            int branchId);

        Task<IDictionary<string, IList<string>>> GetAtLeastNecessaryPermission(int tenantId, string tenantCode,
            long userId, int branchId);

        Task<List<int>> GetAuthorizedBranch(int tenantId, string tenantCode, long userId, bool isAdmin);
    }
}
