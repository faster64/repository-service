using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient.Dtos;
using Newtonsoft.Json;
using KiotVietTimeSheet.Utilities;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.Domain.Common;
using System.Linq;

namespace KiotVietTimeSheet.BackgroundTasks.Infras.KiotVietInternalService.Impls
{
    public class KiotVietInternalService : IKiotVietInternalService
    {
        private readonly HttpClient _httpClient;

        public KiotVietInternalService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<int>> GetAuthorizedBranch(
            int tenantId,
            string tenantCode,
            long userId,
            bool isAdmin,
            CancellationToken ct)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new
            {
                RetailerId = tenantId,
                UserId = userId,
                IsAdmin = isAdmin
            }), Encoding.UTF8, "application/json");

            var resultData = await _httpClient.PostAsync($"api/internal/authorizedBranch", content, ct);
            resultData.EnsureSuccessStatusCode();
            return await resultData.Content.ReadAsJsonAsync<List<int>>();
        }

        public async Task<List<ProductDto>> GetListProductByCategoryId(GetProductByCategoryIdReq req)
        {
            var result = await _httpClient.GetAsync($"api/internal/products/categoryId?CategoryId={req.CategoryId}&RetailerId={req.RetailerId}");
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsJsonAsync<List<ProductDto>>();
        }

        public async Task<IDictionary<string, IList<string>>> GetAtLeastNecessaryPermission(
            int tenantId,
            string tenantCode,
            long userId,
            int branchId,
            CancellationToken ct)
        {
            var contentData = new StringContent(JsonConvert.SerializeObject(new
            {
                RetailerId = tenantId,
                UserId = userId,
                BranchId = branchId,
            }), Encoding.UTF8, "application/json");
            var resultData = await _httpClient.PostAsync($"api/internal/atLeastNecessaryPermission", contentData, ct);
            resultData.EnsureSuccessStatusCode();
            return await resultData.Content.ReadAsJsonAsync<IDictionary<string, IList<string>>>();
        }

        public async Task CreateCommissionSync(GetProductByCategoryIdReq req)
        {
            var payload = new StringContent(JsonConvert.SerializeObject(new
            {
                BranchId = req.BranchId,
                RetailerId = req.RetailerId,
                UserId = req.UserId,
                CommissionDetails = req.CommissionDetails
            }), Encoding.UTF8, "application/json");

            await _httpClient.PostAsync($"api/internal/commission/create-commission-sync", payload);
        }

        public async Task<UserAllPrivilegeDto> GetPermissionByCurrentUser(
            int tenantId,
            string tenantCode,
            long userId,
            int branchId,
            CancellationToken ct)
        {
            var payload = new StringContent(JsonConvert.SerializeObject(new
            {
                RetailerId = tenantId,
                UserId = userId,
                BranchId = branchId,
            }), Encoding.UTF8, "application/json");

            var result = await _httpClient.PostAsync($"api/internal/userPrivilegeBranch", payload, ct);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsJsonAsync<UserAllPrivilegeDto>();
        }

        public async Task<List<Application.ServiceClients.Dtos.PayslipTotalPaymentDto>> GetTotalPaymentByPayslipId(Application.ServiceClients.Dtos.GetTotalPaymentByPayslipIdsReq req, int groupId = 0, string retailerCode = "")
        {
            var payload = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync($"api/internal/payslip-payment/get-total-payment-by-payslips", payload);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsJsonAsync<List<Application.ServiceClients.Dtos.PayslipTotalPaymentDto>>();
        }

        public Task WriteAuditLogAsync(WriteAuditLogRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<BranchDto> GetBranchByIdAsync(int branchId, int tenantId)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<BranchDto>> GetBranchByIdsAsync(List<int> branchIds, int tenantId)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<UserDto>> GetUserByIdsAsync(List<long> ids, int tenantId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<UserByRevenueObject>> GetUserByRevenue(int tenantId, List<int> branchIds, List<long> employeeIds, DateTime startDate, DateTime endDate)
        {
            var payload = new StringContent(JsonConvert.SerializeObject(new GetUserByRevenueReq
            {
                RetailerId = tenantId,
                EmployeeIds = employeeIds,
                BranchIds = branchIds,
                StartDate = startDate,
                EndDate = endDate
            }), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync($"api/internal/users/by-revenue", payload);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsJsonAsync<List<UserByRevenueObject>>();
        }

        public async Task<List<UserByRevenueObject>> GetUserByRevenueCounselor(int tenantId, List<int> branchIds, List<long> employeeIds, DateTime startDate, DateTime endDate)
        {
            var payload = new StringContent(JsonConvert.SerializeObject(new GetUserByRevenueReq
            {
                RetailerId = tenantId,
                EmployeeIds = employeeIds,
                BranchIds = branchIds,
                StartDate = startDate,
                EndDate = endDate
            }), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync($"api/internal/users/by-revenue-counselor", payload);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsJsonAsync<List<UserByRevenueObject>>();
        }
        public async Task<List<ProductRevenue>> GetProductRevenueByUserCounselor(int tenantId, List<int> branchIds, List<long> employeeIds, DateTime startDate, DateTime endDate, int commissionSetting)
        {
            var payload = new StringContent(JsonConvert.SerializeObject(new GetUserByRevenueReq
            {
                RetailerId = tenantId,
                EmployeeIds = employeeIds,
                BranchIds = branchIds,
                StartDate = startDate,
                EndDate = endDate,
                CommissionSetting = commissionSetting
            }), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync($"api/internal/users/product-revenue-by-counselor", payload);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsJsonAsync<List<ProductRevenue>>();
        }

        public async Task<List<UserByRevenueObject>> GetUserByGrossProfit(int tenantId, List<int> branchIds, List<long> employeeIds, DateTime startDate, DateTime endDate)
        {
            try
            {
                var payload = new StringContent(JsonConvert.SerializeObject(new GetUserByRevenueReq
                {
                    RetailerId = tenantId,
                    EmployeeIds = employeeIds,
                    BranchIds = branchIds,
                    StartDate = startDate,
                    EndDate = endDate
                }), Encoding.UTF8, "application/json");
                var result = await _httpClient.PostAsync($"api/internal/users/by-gross-profit", payload);
                result.EnsureSuccessStatusCode();
                return await result.Content.ReadAsJsonAsync<List<UserByRevenueObject>>();
            }
            catch
            {
                //Trường hợp chưa golive MHQL sẽ không ảnh hưởng đến tạo bảng lương
                return new List<UserByRevenueObject>();
            }
        }

        public async Task<List<ProductRevenue>> GetProductRevenueByUser(int tenantId, List<int> branchIds, List<long> employeeIds, DateTime startDate, DateTime endDate, int commissionSetting)
        {
            var payload = new StringContent(JsonConvert.SerializeObject(new GetUserByRevenueReq
            {
                RetailerId = tenantId,
                EmployeeIds = employeeIds,
                BranchIds = branchIds,
                StartDate = startDate,
                EndDate = endDate,
                CommissionSetting = commissionSetting
            }), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync($"api/internal/users/product-revenue", payload);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsJsonAsync<List<ProductRevenue>>();
        }

        
        public async Task<List<ProductRevenue>> GetBranchProductRevenues(int tenantId, string tenantCode, bool isAllBranch, List<BranchDto> branchesDto, IList<long> branchIds, DateTime startDate, DateTime endDate)
        {
            if (isAllBranch)
            {
                branchIds = branchesDto.Select(x => x.Id).ToList();
            }

            var payload = new StringContent(JsonConvert.SerializeObject(new GetBranchRevenue
            {
                RetailerId = tenantId,
                BranchIds = branchIds,
                StartDate = startDate,
                EndDate = endDate.Date.AddDays(1).AddTicks(-1)
            }), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync($"api/internal/branch/reneuve", payload);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsJsonAsync<List<ProductRevenue>>();
        }
       
    }
}
