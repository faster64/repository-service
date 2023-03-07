using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionEvents;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;

namespace KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types
{
    public class CommissionAuditProcess : BaseAuditProcess
    {
        public CommissionAuditProcess(
            IKiotVietInternalService kiotVietInternalService
        ) : base(kiotVietInternalService)
        {}

        public async Task WriteCreateCommissionLogAsync(CreatedCommissionIntegrationEvent @event)
        {
            var content = await RenderCreateCommissionAuditlog(@event.Commission, @event.BranchIds);

            await AddLogAsync(
                GenerateLog(
                    TimeSheetFunctionTypes.CommissionManagement,
                    TimeSheetAuditTrailAction.Create,
                    content,
                    @event.Context
                )
            );
        }

        public async Task WriteUpdateCommissionLogAsync(UpdatedCommissionIntegrationEvent @event)
        {
            var content = await RenderUpdateCommissionAuditlog(@event.Commission, @event.BranchIds);
            await AddLogAsync(
                GenerateLog(
                    TimeSheetFunctionTypes.CommissionManagement,
                    TimeSheetAuditTrailAction.Update,
                    content,
                    @event.Context
                )
            );
        }

        public async Task WriteDeleteCommissionLogAsync(DeletedCommissionIntegrationEvent @event)
        {
            var content = $"Xóa bảng hoa hồng: {@event.Commission.Name}";
            await AddLogAsync(
                GenerateLog(
                    TimeSheetFunctionTypes.CommissionManagement,
                    TimeSheetAuditTrailAction.Delete,
                    content,
                    @event.Context
                )
            );
        }

        private async Task<string> RenderCreateCommissionAuditlog(Commission commission, List<int> branchIds)
        {
            var logCreateContent = await RenderAuditLog(commission, branchIds);
            return "Thêm mới bảng hoa hồng:" + logCreateContent;
        }

        private async Task<string> RenderUpdateCommissionAuditlog(Commission commission, List<int> branchIds)
        {
            var logUpdateContent = await RenderAuditLog(commission, branchIds);
            return "Cập nhật bảng hoa hồng:" + logUpdateContent;
        }

        private async Task<string> RenderAuditLog(Commission commission, List<int> branchIds)
        {
            string scope;
            if (branchIds.Any() || branchIds.Count > 0)
            {
                var branches = await KiotVietInternalService.GetBranchByIdsAsync(branchIds, commission.TenantId);
                scope = string.Join(", ", branches.Select(x => x.Name).ToList());
            }
            else
            {
                scope = "Toàn hệ thống";
            }

            var status = commission.IsActive ? "Áp dụng" : "Ngừng hoạt động";
            return $"<br/>- Tên: " + commission.Name +
                   $"<br/>- Phạm vi áp dụng: {scope}" +
                   $"<br/>- Trạng thái: {status}";
        }
    }
}
