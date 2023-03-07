using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.Common;
using ServiceStack;
using KiotVietTimeSheet.Infrastructure.Configuration;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient.Dtos;
using KiotVietTimeSheet.SharedKernel.Models;
using Newtonsoft.Json;
using System.Threading;
using KiotVietTimeSheet.Application.Queries.GetUserAccount;

namespace KiotVietTimeSheet.Infrastructure.KiotVietApiClient
{
    public class KiotVietApiClient : IKiotVietApiClient, IKiotVietServiceClient
    {

        protected readonly KiotVietApiClientContext Context;

        public KiotVietApiClient(KiotVietApiClientContext context)
        {
            Context = context;
        }

        public async Task<UserPrivilegeDto> GetPermissionByBranchId(long userId, int brandId)
        {
            string url = string.Format("users/{0}/privileges?BranchId={1}", userId, brandId);
            return await SendAsync<UserPrivilegeDto>(HttpMethod.Get, url);
        }

        public async Task<BranchDto> GetBranchById(int branchId)
        {
            string url = string.Format("branchs/{0}", branchId);
            return await SendAsync<BranchDto>(HttpMethod.Get, url);
        }

        public async Task<PagingDataSource<BranchDto>> GetBranch()
        {
            return await SendAsync<PagingDataSource<BranchDto>>(HttpMethod.Get, "branchs");
        }

        public async Task<PagingDataSource<BranchDto>> GetBranchByPermission(string permission)
        {
            return await SendAsync<PagingDataSource<BranchDto>>(HttpMethod.Get, $"branchs?permission={permission}");
        }

        public async Task<PagingDataSource<BranchMobileDto>> GetBranchByPermissionForMobile(string permission)
        {
            return await SendAsync<PagingDataSource<BranchMobileDto>>(HttpMethod.Get, $"branchs?permission={permission}");
        }

        public async Task<List<PayslipPaymentAllocationDto>> GetPayslipPaymentsValueIncludeAllocation(GetPayslipPaymentsValueIncludeAllocationReq req)
        {
            return await SendAsync<List<PayslipPaymentAllocationDto>>(HttpMethod.Get,
                $"payslip-payments/get-allocation-value?EmployeeIds={req.EmployeeIds.Join(",")}");
        }

        public async Task<List<PayslipPaymentDto>> MakePayslipPaymentsForPaysheetAsync(MakePayslipPaymentsForPaysheetReq req)
        {
            return await SendAsync<List<PayslipPaymentDto>>(HttpMethod.Post,
                $"payslip-payment/make-payments-for-paysheet", SerializeToJsonContent(req));
        }

        public async Task<List<PayslipPaymentDto>> VoidPayslipPaymentsAsync(VoidPayslipPaymentReq req)
        {
            return await SendAsync<List<PayslipPaymentDto>>(HttpMethod.Post, $"payslip-payment/void-payments",
                SerializeToJsonContent(req));
        }

        public async Task<PagingDataSource<PayslipPaymentDto>> GetPayslipPaymentPaidPayslipAsync(GetPaidPayslipPaymentReq req)
        {
            return await SendAsync<PagingDataSource<PayslipPaymentDto>>(HttpMethod.Get, $"payslip-payments/paid-payslip-payments?{GetQueryString(req)}");
        }

        public async Task<PagingDataSource<PayslipPaymentDto>> GetPayslipPaymentsAsync(GetPayslipPaymentsReq req)
        {
            return await SendAsync<PagingDataSource<PayslipPaymentDto>>(HttpMethod.Get,
                $"payslip-payments?{GetQueryString(req)}");
        }

        public async Task<List<UserByRevenueObject>> GetUserByRevenue(
            int tenantId,
            List<int> branchIds,
            List<long> employeeIds,
            DateTime startDate,
            DateTime endDate)
        {
            return await SendInternalAsync<List<UserByRevenueObject>>(HttpMethod.Post, $"internal/users/by-revenue",
                SerializeToJsonContent(new GetUserByRevenueReq
                {
                    RetailerId = tenantId,
                    EmployeeIds = employeeIds,
                    BranchIds = branchIds,
                    StartDate = startDate,
                    EndDate = endDate
                }));
        }

        public async Task<List<UserByRevenueObject>> GetUserByGrossProfit(
            int tenantId,
            List<int> branchIds,
            List<long> employeeIds,
            DateTime startDate,
            DateTime endDate)
        {
            try
            {
                return await SendInternalAsync<List<UserByRevenueObject>>(HttpMethod.Post, $"internal/users/by-gross-profit",
                    SerializeToJsonContent(new GetUserByRevenueReq
                    {
                        RetailerId = tenantId,
                        EmployeeIds = employeeIds,
                        BranchIds = branchIds,
                        StartDate = startDate,
                        EndDate = endDate
                    }));
            } 
            catch
            {
                //Trường hợp chưa golive MHQL sẽ không ảnh hưởng đến tạo bảng lương
                return new List<UserByRevenueObject>();
            }
        }

        public async Task<List<ProductDto>> GetProductByCategoryId(GetProductByCategoryIdReq req)
        {
            return await SendAsync<List<ProductDto>>(HttpMethod.Get,
                $"/products/categoryId?CategoryId={req.CategoryId}");
        }

        public async Task<List<ProductDto>> GetProductByCategoryIdDataTrial(GetProductByCategoryIdReq req)
        {
            return await SendInternalAsync<List<ProductDto>>(HttpMethod.Get, $"/internal/products/categoryId?CategoryId={req.CategoryId}&RetailerId={req.RetailerId}", groupId: req.GroupId);
        }

        public async Task<PagingDataSource<ProductCommissionDetailDto>> GetTimeSheetProductCommissionReq(GetTimeSheetProductCommissionReq req)
        {
            return await SendAsync<PagingDataSource<ProductCommissionDetailDto>>(HttpMethod.Get,
                $"/products/get-time-sheet-product-commission?{GetQueryString(req)}");
        }

        public async Task<List<ProductRevenue>> GetProductRevenueByUser(
            int tenantId,
            List<int> branchIds,
            List<long> employeeIds,
            DateTime startDate,
            DateTime endDate)
        {
            return await SendInternalAsync<List<ProductRevenue>>(HttpMethod.Post, $"internal/users/product-revenue",
                SerializeToJsonContent(new GetUserByRevenueReq
                {
                    RetailerId = tenantId,
                    EmployeeIds = employeeIds,
                    BranchIds = branchIds,
                    StartDate = startDate,
                    EndDate = endDate
                }));
        }
      
        public async Task CreateCommissionDetailSync(GetProductByCategoryIdReq req)
        {
            await SendInternalAsync<object>(HttpMethod.Post, $"internal/commission/create-commission-sync",
                SerializeToJsonContent(new
                {
                    BranchId = req.BranchId,
                    RetailerId = req.RetailerId,
                    UserId = req.UserId,
                    CommissionDetails = req.CommissionDetails
                }), req.GroupId);
        }
      
        public async Task<object> ReCreateCommissionDetailSync(GetProductByCategoryIdReq req, SharedKernel.Auth.ExecutionContext context = null)
        {
            if (context != null)
            {
                req.BranchId = context.BranchId;
                req.RetailerId = context.TenantId;
                req.UserId = context.User.Id;
                req.GroupId = context.User.GroupId;
            }
            return await SendInternalAsync<object>(HttpMethod.Post,
                "internal/commission/recreate-commission-sync",
                SerializeToJsonContent(new
                {
                    req.BranchId,
                    req.RetailerId,
                    req.UserId,
                    req.CommissionDetails
                }), req.GroupId);
        }

        public async Task CreateCommissionDetailWhenCreateOrUpdateProductSync(
            List<CommissionDetailDto> commissionDetails,
            int branchId,
            int tenantId,
            long userId)
        {
            await SendInternalAsync<object>(HttpMethod.Post, $"internal/commission/create-commission-sync",
                SerializeToJsonContent(new
                {
                    BranchId = branchId,
                    RetailerId = tenantId,
                    UserId = userId,
                    CommissionDetails = commissionDetails,
                }));
        }

        public async Task DeleteCommissionSync(GetProductByCategoryIdReq req)
        {
            await SendInternalAsync<object>(HttpMethod.Post, $"internal/commission/delete-commission-sync",
                SerializeToJsonContent(new
                {
                    BranchId = req.BranchId,
                    RetailerId = req.RetailerId,
                    UserId = req.UserId,
                    CommissionDetails = req.CommissionDetails
                }), req.GroupId, req.RetailerCode);
        }
        public async Task DeleteTimeSheetCommissionSync(GetProductByCategoryIdReq req, long commissionId)
        {
            await SendInternalAsync<object>(HttpMethod.Post, $"internal/commission/delete-timesheetcommission-sync",
               SerializeToJsonContent(new
               {
                   BranchId = req.BranchId,
                   RetailerId = req.RetailerId,
                   UserId = req.UserId,
                   CommissionId = commissionId
               }), req.GroupId, req.RetailerCode);
        }

        public async Task<List<ProductRevenue>> GetBranchRevenue(
            int tenantId,
            string tenantCode,
            IList<long> branchIds,
            DateTime startDate,
            DateTime endDate
        )
        {
            return await SendInternalAsync<List<ProductRevenue>>(HttpMethod.Post, $"internal/branch/reneuve",
                SerializeToJsonContent(new
                {
                    TenantId = tenantId,
                    BranchIds = branchIds,
                    StartDate = startDate,
                    EndDate = endDate
                }), null, tenantCode);
        }

        public async Task<TimeSheetPosParam> GetTimeSheetPosParam(int retailerId, string retailerCode, CancellationToken ct)
        {
            return await SendInternalAsync<TimeSheetPosParam>(HttpMethod.Get,
                $"internal/timeSheetPosParam?RetailerId={retailerId}", null, null, retailerCode, ct);
        }

        public async Task<ImportExportFile> AddImportExportFile(InternalImportExportReq req)
        {
            var request = SerializeToJsonContent(req);
            return await SendInternalAsync<ImportExportFile>(HttpMethod.Post, "internal/addimportexport",
                request);
        }

        private async Task<T> SendAsync<T>(
            HttpMethod method,
            string path,
            HttpContent content = null,
            int? groupId = null,
            CancellationToken ct = default
        )
        {
            var client = HostContext.Resolve<HttpClient>();
            var request = new HttpRequestMessage(method, $"{GetEndPoint(groupId)}/{path}");
            if (method == HttpMethod.Post && content != null)
            {
                request.Content = content;
            }
            request.Headers.Accept.Clear();
            request.Headers.Add(InfrastructureConfiguration.RequestHeaderRetailer, Context.RetailerCode);
            request.Headers.Add(InfrastructureConfiguration.RequestHeaderAuthorization, $"{InfrastructureConfiguration.AuthenticationSchemeType} {Context.BearerToken}");
            request.Headers.Add(InfrastructureConfiguration.RequestHeaderBranchId, Context.BranchId.ToString());
            request.Headers.Add(InfrastructureConfiguration.RequestHeaderKvVersion, Context.Fnb.KvVersion);
            request.Headers.Add(InfrastructureConfiguration.RequestHeaderXGroupId, Context.GroupId.ToString());
            request.Headers.Add(InfrastructureConfiguration.RequestHeaderXRetailerCode, Context.RetailerCode);
            request.Headers.Add("App", "timesheet");
            var response = await client.SendAsync(request, ct);
            var result = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            return result;
        }

        private async Task<T> SendInternalAsync<T>(
            HttpMethod method,
            string path,
            HttpContent content = null,
            int? groupId = null,
            string retailerCode = "",
            CancellationToken ct = default
        )
        {
            var client = HostContext.Resolve<HttpClient>();
            var request = new HttpRequestMessage(method, $"{GetEndPoint(groupId)}/{path}");
            if (method == HttpMethod.Post && content != null)
            {
                request.Content = content;
            }
            var token = Context.Retail.InternalToken;
            if (IsFnB(groupId)) token = Context.Fnb.InternalToken;
            else if (IsBooking(groupId)) token = Context.Booking.InternalToken;
            if (string.IsNullOrEmpty(retailerCode))
            {
                retailerCode = Context.RetailerCode;
            }
            request.Headers.Accept.Clear();
            request.Headers.Add("InternalApiToken", token);
            request.Headers.Add(InfrastructureConfiguration.RequestHeaderRetailer, retailerCode);
            request.Headers.Add(InfrastructureConfiguration.RequestHeaderXGroupId, Context.GroupId.ToString());
            request.Headers.Add(InfrastructureConfiguration.RequestHeaderXRetailerCode, Context.RetailerCode);
            request.Headers.Add("App", "timesheet");
            var response = await client.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            return result;

        }

        private StringContent SerializeToJsonContent(object obj)
        {
            return new StringContent(obj.ToSafeJson(), Encoding.UTF8, "application/json");
        }

        private string GetEndPoint(int? groupId = null)
        {
            if (groupId == null)
                groupId = Context.GroupId;

            string endPoint;
            if (groupId == null)
            {
                endPoint = string.Empty;
            }
            else if (IsFnB(groupId))
            {
                endPoint = Context.Fnb.EndPoint;
            }
            else
            {
                endPoint = Context.Booking?.EndPoint;
            }

            if (string.IsNullOrEmpty(endPoint))
            {
                var ex = new Exception("Không xác định endpoint");
                throw ex;
            }
            return endPoint;
        }

        private bool IsFnB(int? groupId = null, int? industryId = null)
        {
            if (groupId == null)
                groupId = Context.GroupId;

            if (industryId == null)
                industryId = Context.IndustryId;

            return groupId != null && (Context.Fnb.GroupIds.Contains(groupId.GetValueOrDefault()) || industryId == 15);
        }

        private bool IsBooking(int? groupId = null, int? industryId = null)
        {
            try
            {
                if (groupId == null)
                    groupId = Context.GroupId;

                if (industryId == null)
                    industryId = Context.IndustryId;

                if (Context.Booking == null || Context.Booking.GroupIds == null || Context.Booking.GroupIds.Length == 0) return false;

                return groupId != null && (Context.Booking.GroupIds.Contains(groupId.GetValueOrDefault()) || industryId == 16);
            }
            catch (Exception)
            {
                return false;
            }            
        }

        public string GetQueryString(object obj)
        {
            var properties = obj.GetType().GetProperties()
                .Where(x => x.CanRead)
                .Where(x => x.GetValue(obj, null) != null)
                .ToDictionary(x => x.Name, x => x.GetValue(obj, null));

            var propertyNames = properties
                .Where(x => !(x.Value is string) && x.Value is IList)
                .Select(x => x.Key)
                .ToList();

            foreach (var key in propertyNames)
            {
                var valueType = properties[key].GetType();
                var valueElemType = valueType.IsGenericType
                    ? valueType.GetGenericArguments()[0]
                    : valueType.GetElementType();
                if (valueElemType == null || (!valueElemType.IsPrimitive && valueElemType != typeof(string))) continue;
                var iList = properties[key] as IList;
                properties[key] = string.Join(",", (iList ?? throw new InvalidOperationException()).Cast<object>());
            }

            return string.Join("&", properties
                .Select(x => string.Concat(
                    Uri.EscapeDataString(x.Key), "=",
                    Uri.EscapeDataString(x.Value.ToString()))));
        }

        public async Task<UserAccountDto> GetUserAccount(GetUserAccountQuery req)
        {
            return await SendAsync<UserAccountDto>(HttpMethod.Get, "/setting/account");
        }

        public async Task<InternalResponseDto> OnChangeEmployee(OnChangeEmployeeReq req, CancellationToken ct)
        {
            return await SendInternalAsync<InternalResponseDto>(HttpMethod.Post, $"internal/employee",
               SerializeToJsonContent(req));
        }

        public async Task<InternalResponseDto> OnDeleteEmployee(OnDelEmployeeReq req, CancellationToken ct)
        {            
            return await SendInternalAsync<InternalResponseDto>(HttpMethod.Delete, $"internal/employee?RetailerId={req.RetailerId}&EmployeeId={req.EmployeeId}", SerializeToJsonContent(req));
        }
    }
}
