using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.Infrastructure.AuditTrail;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using Microsoft.EntityFrameworkCore;
using ServiceStack.Configuration;

namespace KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types
{
    public class CommissionDetailAuditProcess : BaseAuditProcess
    {
        private readonly IAppSettings _appSettings;
        private EfDbContext _db;
        private readonly IKiotVietApiClient _kiotVietApiClient;
        private readonly ICacheClient _cacheClient;
        private readonly Helper _helper = new Helper();
        public CommissionDetailAuditProcess(
            IKiotVietApiClient kiotVietApiClient,
            IAuditProcessFailEventService auditProcessFailEventService,
            IAppSettings appSettings, 
            ICacheClient cacheClient
            )
            : base(kiotVietApiClient, auditProcessFailEventService)
        {
            _kiotVietApiClient = kiotVietApiClient;
            _appSettings = appSettings;
            _cacheClient = cacheClient;
        }

        public async Task WriteCreateCommissionDetailByProductLogAsync(CreatedCommissionDetailByProductIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    using (_db = await _helper.GetDbContextByGroupId(@event.Context.GroupId, @event.Context.RetailerCode, _cacheClient, _kiotVietApiClient, _appSettings))
                    {
                        var commissionDetails = @event.ListCommissionDetails;
                        var commissionIds = commissionDetails.Select(x => x.CommissionId).Distinct().ToList();
                        var commissionName = await RenderCommissionNameAsync(commissionIds);

                        var product = @event.Product;
                        var productCode = product.Code;

                        var content = $"Thêm hàng hóa [ProductCode]{productCode}[/ProductCode] vào bảng hoa hồng {commissionName}";
                        await AddLogAsync(
                            GenerateLog(
                                TimeSheetFunctionTypes.CommissionManagement,
                                TimeSheetAuditTrailAction.Create,
                                content,
                                @event.Context
                            ),
                            @event.Context.GroupId, @event.Context.RetailerCode
                        );
                    }

                }
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        public async Task WriteCreateCommissionDetailByProductCategoryLogAsync(CreatedCommissionDetailByProductCategoryIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    using (_db = await _helper.GetDbContextByGroupId(@event.Context.GroupId,
                        @event.Context.RetailerCode, _cacheClient, _kiotVietApiClient, _appSettings))
                    {
                        var commissionIds = @event.ListCommissionDetails.Select(c => c.CommissionId).Distinct()
                            .ToList();
                        var commissionName = await RenderCommissionNameAsync(commissionIds);

                        var productCategory = @event.ProductCategory;
                        var productCategoryName = productCategory.Name;

                        var content =
                            $"Thêm nhóm hàng {productCategoryName} vào bảng hoa hồng {commissionName}";
                        await AddLogAsync(
                            GenerateLog(
                                TimeSheetFunctionTypes.CommissionManagement,
                                TimeSheetAuditTrailAction.Create,
                                content,
                                @event.Context
                            ),
                            @event.Context.GroupId, @event.Context.RetailerCode
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        public async Task WriteCreateCommissionDetailByProductCategoryAsyncLogAsync(CreatedCommissionDetailByProductCategoryAsyncIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    using (_db = await _helper.GetDbContextByGroupId(@event.Context.GroupId,
                        @event.Context.RetailerCode, _cacheClient, _kiotVietApiClient, _appSettings))
                    {
                        var commissionName = await RenderCommissionNameAsync(@event.CommissionIds);

                        var productCategory = @event.ProductCategory;
                        var productCategoryName = productCategory.Name;

                        var content =
                            $"Thêm nhóm hàng {productCategoryName} vào bảng hoa hồng {commissionName}";
                        await AddLogAsync(
                            GenerateLog(
                                TimeSheetFunctionTypes.CommissionManagement,
                                TimeSheetAuditTrailAction.Create,
                                content,
                                @event.Context
                            ),
                            @event.Context.GroupId, @event.Context.RetailerCode
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        public async Task WriteUpdateValueOfCommissionDetailLogAsync(UpdatedValueOfCommissionDetailIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    using (_db = await _helper.GetDbContextByGroupId(@event.Context.GroupId,
                        @event.Context.RetailerCode, _cacheClient, _kiotVietApiClient, _appSettings))
                    {
                        var commissionDetails = @event.ListCommissionDetails;
                        var commissionIds = commissionDetails.Select(x => x.CommissionId).Distinct().ToList();
                        var commissionName = await RenderCommissionNameAsync(commissionIds);

                        var products = @event.Products;
                        var content = "";
                        if (commissionDetails.Count >= 1)
                        {
                            // TH cập nhật nhiều sản phẩm
                            if (commissionDetails.Count > 1)
                            {
                                content =
                                    $"Cập nhật mức hoa hồng của {commissionDetails.Count} hàng hóa trên bảng hoa hồng {commissionName}";
                            }
                            // TH chỉ cập nhật 1 sản phẩm
                            else
                            {
                                var product = products.Select(x => x.Code).FirstOrDefault();
                                content =
                                    $"Cập nhật mức hoa hồng của hàng hóa {product} trên bảng hoa hồng {string.Join(", ", commissionName)}";
                            }
                        }

                        await AddLogAsync(
                            GenerateLog(
                                TimeSheetFunctionTypes.CommissionManagement,
                                TimeSheetAuditTrailAction.Update,
                                content,
                                @event.Context
                            ),
                            @event.Context.GroupId, @event.Context.RetailerCode
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        public async Task WriteDeleteCommissionDetailLogAsync(DeletedCommissionDetailIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    using (_db = await _helper.GetDbContextByGroupId(@event.Context.GroupId,
                        @event.Context.RetailerCode, _cacheClient, _kiotVietApiClient, _appSettings))
                    {
                        var commissionDetails = @event.ListCommissionDetails;
                        var commissionIds = commissionDetails.Select(x => x.CommissionId).Distinct().ToList();
                        var commissionName = await RenderCommissionNameAsync(commissionIds);

                        var products = @event.Products;
                        var listProduct = RenderListProduct(products);

                        var content = $"Xóa các hàng hóa khỏi bảng hoa hồng {commissionName}:" +
                                      $"{listProduct}";
                        await AddLogAsync(
                            GenerateLog(
                                TimeSheetFunctionTypes.CommissionManagement,
                                TimeSheetAuditTrailAction.Delete,
                                content,
                                @event.Context
                            ),
                            @event.Context.GroupId, @event.Context.RetailerCode
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        private async Task<string> RenderCommissionNameAsync(List<long> commissionIds)
        {
            var commission = await _db.Commission.Where(x => commissionIds.Contains(x.Id)).Select(x => x.Name).ToListAsync();
            return string.Join(", ", commission);
        }

        private string RenderListProduct(List<ProductCommissionDetailDto> products)
        {
            var listProduct = new StringBuilder();
            foreach (var product in products)
            {
                var productCode = product.Code;
                var productName = product.Name;
                listProduct.Append($"<br/>- {productCode}" + $", {productName}");
            }

            return listProduct.ToString();
        }
    }
}
