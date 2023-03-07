using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Queries.GetUserAccount;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.ServiceClients
{
    public interface IKiotVietServiceClient
    {
        Task<List<UserByRevenueObject>> GetUserByRevenue(
            int tenantId,
            List<int> branchIds,
            List<long> employeeIds,
            DateTime startDate,
            DateTime endDate
        );

        Task<List<UserByRevenueObject>> GetUserByGrossProfit(
            int tenantId,
            List<int> branchIds,
            List<long> employeeIds,
            DateTime startDate,
            DateTime endDate
        );

        Task<UserPrivilegeDto> GetPermissionByBranchId(long userId, int brandId);

        Task<List<PayslipPaymentAllocationDto>> GetPayslipPaymentsValueIncludeAllocation(
            GetPayslipPaymentsValueIncludeAllocationReq req);
        Task<List<PayslipPaymentDto>> MakePayslipPaymentsForPaysheetAsync(MakePayslipPaymentsForPaysheetReq req);

        Task<List<PayslipPaymentDto>> VoidPayslipPaymentsAsync(VoidPayslipPaymentReq req);

        Task<PagingDataSource<PayslipPaymentDto>> GetPayslipPaymentPaidPayslipAsync(GetPaidPayslipPaymentReq req);

        Task<PagingDataSource<PayslipPaymentDto>> GetPayslipPaymentsAsync(GetPayslipPaymentsReq req);

        Task<List<ProductDto>> GetProductByCategoryId(GetProductByCategoryIdReq req);
        Task<List<ProductDto>> GetProductByCategoryIdDataTrial(GetProductByCategoryIdReq req);

        Task<PagingDataSource<ProductCommissionDetailDto>> GetTimeSheetProductCommissionReq(GetTimeSheetProductCommissionReq req);

        Task<PagingDataSource<BranchDto>> GetBranchByPermission(string permission);

        Task<PagingDataSource<BranchMobileDto>> GetBranchByPermissionForMobile(string permission);

        Task<PagingDataSource<BranchDto>> GetBranch();
        Task<BranchDto> GetBranchById(int branchId);

        Task CreateCommissionDetailWhenCreateOrUpdateProductSync(
            List<CommissionDetailDto> commissionDetails,
            int branchId,
            int tenantId,
            long userId);

        Task DeleteCommissionSync(GetProductByCategoryIdReq req);

        Task DeleteTimeSheetCommissionSync(GetProductByCategoryIdReq req,long commissionId);

        Task<List<ProductRevenue>> GetProductRevenueByUser(
            int tenantId,
            List<int> branchIds,
            List<long> employeeIds,
            DateTime startDate,
            DateTime endDate
        );

        Task CreateCommissionDetailSync(GetProductByCategoryIdReq req);
        Task<object> ReCreateCommissionDetailSync(GetProductByCategoryIdReq req, SharedKernel.Auth.ExecutionContext context = null);

        Task<List<ProductRevenue>> GetBranchRevenue(
            int tenantId,
            string tenantCode,
            IList<long> branchIds,
            DateTime startDate,
            DateTime endDate
        );

        Task<TimeSheetPosParam> GetTimeSheetPosParam(int retailerId, string retailerCode, CancellationToken ct);
        Task<UserAccountDto> GetUserAccount(GetUserAccountQuery req);
        Task<ImportExportFile> AddImportExportFile(InternalImportExportReq req);
        Task<InternalResponseDto> OnChangeEmployee(OnChangeEmployeeReq req, CancellationToken ct);
        Task<InternalResponseDto> OnDeleteEmployee(OnDelEmployeeReq req, CancellationToken ct);

    }
}
