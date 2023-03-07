using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents;
using KiotVietTimeSheet.Application.Queries.GetCommissionByIds;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Common;
using MediatR;

namespace KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types
{
    public class CommissionDetailAuditProcess : BaseAuditProcess
    {
        private readonly IMediator _mediator;
        public CommissionDetailAuditProcess(
            IKiotVietInternalService kiotVietInternalService,
            IMediator mediator
        ) : base(kiotVietInternalService)
        {
            _mediator = mediator;
        }

        public async Task WriteCreateCommissionDetailByProductLogAsync(CreatedCommissionDetailByProductIntegrationEvent @event)
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
                )
            );
        }

        public async Task WriteCreateCommissionDetailByProductCategoryLogAsync(CreatedCommissionDetailByProductCategoryIntegrationEvent @event)
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
                )
            );
        }

        public async Task WriteCreateCommissionDetailByProductCategoryAsyncLogAsync(CreatedCommissionDetailByProductCategoryAsyncIntegrationAuditEvent @event)
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
                )
            );
        }

        public async Task WriteUpdateValueOfCommissionDetailLogAsync(UpdatedValueOfCommissionDetailIntegrationEvent @event)
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
                )
            );
        }

        public async Task WriteDeleteCommissionDetailLogAsync(DeletedCommissionDetailIntegrationEvent @event)
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
                )
            );
        }

        public async Task WriteDeleteCommissionDetailLogAsync(DeletedCommissionDetailIntegrationAuditEvent @event)
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
                )
            );
        }

        public async Task WriteCreateCommissionDetailByProductAuditLogAsync(CreatedCommissionDetailByProductIntegrationAuditEvent @event)
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
                )
            );
        }

        private async Task<string> RenderCommissionNameAsync(List<long> commissionIds)
        {
            var commissionTmp = await _mediator.Send(new GetCommissionByIdsQuery(commissionIds));
            var commission = commissionTmp.Select(x => x.Name).ToList();

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
