using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using Newtonsoft.Json;
using KiotVietTimeSheet.Utilities;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using System;
using KiotVietTimeSheet.Domain.Common;

namespace KiotVietTimeSheet.AuditTrailWorker.Infras.KiotVietInternalService.Impls
{
    public class KiotVietInternalService : IKiotVietInternalService
    {
        private readonly HttpClient _httpClient;

        public KiotVietInternalService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BranchDto> GetBranchByIdAsync(int branchId, int tenantId)
        {
            var payload = new StringContent(JsonConvert.SerializeObject(new
            {
                Id = branchId,
                RetailerId = tenantId
            }), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync($"api/internal/branchs/{branchId}", payload);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsJsonAsync<BranchDto>();
        }

        public async Task<List<BranchDto>> GetBranchByIdsAsync(List<int> branchIds, int tenantId)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(new
            {
                Ids = branchIds,
                RetailerId = tenantId
            }), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync($"api/internal/branchs/getByIds", httpContent);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsJsonAsync<List<BranchDto>>();
        }

        public async Task<List<UserDto>> GetUserByIdsAsync(List<long> ids, int tenantId)
        {
            var payload = new StringContent(JsonConvert.SerializeObject(new
            {
                Ids = ids,
                RetailerId = tenantId
            }), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync($"api/internal/users/getByIds", payload);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsJsonAsync<List<UserDto>>();
        }

        public async Task WriteAuditLogAsync(WriteAuditLogRequest request)
        {
            var payload = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync("api/internal/audit-trail/write", payload);
            result.EnsureSuccessStatusCode();
        }

        public async Task<IDictionary<string, IList<string>>> GetAtLeastNecessaryPermission(
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
            var result = await _httpClient.PostAsync($"api/internal/atLeastNecessaryPermission", payload, ct);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsJsonAsync<IDictionary<string, IList<string>>>();
        }

        public async Task<List<int>> GetAuthorizedBranch(
            int tenantId,
            string tenantCode,
            long userId,
            bool isAdmin,
            CancellationToken ct)
        {
            var payload = new StringContent(JsonConvert.SerializeObject(new
            {
                RetailerId = tenantId,
                UserId = userId,
                IsAdmin = isAdmin
            }), Encoding.UTF8, "application/json");

            var result = await _httpClient.PostAsync($"api/internal/authorizedBranch", payload, ct);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsJsonAsync<List<int>>();
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


        public Task<List<ProductDto>> GetListProductByCategoryId(GetProductByCategoryIdReq req)
        {
            throw new System.NotImplementedException();
        }
        public Task<List<UserByRevenueObject>> GetUserByRevenue(int tenantId, List<int> branchIds, List<long> employeeIds, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
        
        public Task<List<UserByRevenueObject>> GetUserByGrossProfit(int tenantId, List<int> branchIds, List<long> employeeIds, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }


        public Task CreateCommissionSync(GetProductByCategoryIdReq req)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<ProductRevenue>> GetBranchProductRevenues(int tenantId, string tenantCode, bool isAllBranch, List<BranchDto> branchesDto, IList<long> branchIds, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductRevenue>> GetProductRevenueByUser(int tenantId, List<int> branchIds, List<long> employeeIds, DateTime startDate, DateTime endDate, int commissionSetting)
        {
            throw new NotImplementedException();
        }

        public Task<List<Application.ServiceClients.Dtos.PayslipTotalPaymentDto>> GetTotalPaymentByPayslipId(Application.ServiceClients.Dtos.GetTotalPaymentByPayslipIdsReq req, int groupId = 0, string retailerCode = "")
        {
            throw new System.NotImplementedException();
        }

        public Task<List<UserByRevenueObject>> GetUserByRevenueCounselor(int tenantId, List<int> branchIds, List<long> employeeIds, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductRevenue>> GetProductRevenueByUserCounselor(int tenantId, List<int> branchIds, List<long> employeeIds, DateTime startDate, DateTime endDate, int commissionSetting)
        {
            throw new NotImplementedException();
        }
    }
}
