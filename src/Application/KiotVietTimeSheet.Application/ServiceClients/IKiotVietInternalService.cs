using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.Common;

namespace KiotVietTimeSheet.Application.ServiceClients
{
    public interface IKiotVietInternalService
    {
        Task WriteAuditLogAsync(WriteAuditLogRequest request);

        Task<BranchDto> GetBranchByIdAsync(int branchId, int tenantId);

        Task<List<BranchDto>> GetBranchByIdsAsync(List<int> branchIds, int tenantId);

        Task<List<UserDto>> GetUserByIdsAsync(List<long> ids, int tenantId);

        Task<List<PayslipTotalPaymentDto>> GetTotalPaymentByPayslipId(GetTotalPaymentByPayslipIdsReq req, int groupId = 0, string retailerCode = "");
        Task<List<ProductDto>> GetListProductByCategoryId(GetProductByCategoryIdReq req);
        Task CreateCommissionSync(GetProductByCategoryIdReq req);

        Task<IDictionary<string, IList<string>>> GetAtLeastNecessaryPermission(
            int tenantId,
            string tenantCode,
            long userId,
            int branchId,
            CancellationToken ct);

        Task<UserAllPrivilegeDto> GetPermissionByCurrentUser(
            int tenantId,
            string tenantCode,
            long userId,
            int branchId,
            CancellationToken ct);

        Task<List<int>> GetAuthorizedBranch(
            int tenantId,
            string tenantCode,
            long userId,
            bool isAdmin,
            CancellationToken ct);

        Task<List<UserByRevenueObject>> GetUserByRevenue(int tenantId, List<int> branchIds, List<long> employeeIds, DateTime startDate, DateTime endDate);
        Task<List<UserByRevenueObject>> GetUserByRevenueCounselor(int tenantId, List<int> branchIds, List<long> employeeIds, DateTime startDate, DateTime endDate);

        Task<List<UserByRevenueObject>> GetUserByGrossProfit(int tenantId, List<int> branchIds, List<long> employeeIds, DateTime startDate, DateTime endDate);

        Task<List<ProductRevenue>> GetProductRevenueByUser(int tenantId, List<int> branchIds, List<long> employeeIds, DateTime startDate, DateTime endDate, int commissionSetting);
        Task<List<ProductRevenue>> GetProductRevenueByUserCounselor(int tenantId, List<int> branchIds, List<long> employeeIds, DateTime startDate, DateTime endDate, int commissionSetting);

        Task<List<ProductRevenue>> GetBranchProductRevenues(int tenantId, string tenantCode, bool isAllBranch, List<BranchDto> branchesDto, IList<long> branchIds, DateTime startDate, DateTime endDate);
        
    }
}
