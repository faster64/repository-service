using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionEvents;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.Infrastructure.AuditTrail;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;

namespace KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types
{
    public class CommissionAuditProcess : BaseAuditProcess
    {
        public CommissionAuditProcess(
            IKiotVietApiClient kiotVietApiClient,
            IAuditProcessFailEventService auditProcessFailEventService 
        )
            : base(kiotVietApiClient, auditProcessFailEventService)
        { }

        public async Task WriteCreateCommissionLogAsync(CreatedCommissionIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    var content = await RenderCreateCommissionAuditlog(@event.Commission, @event.BranchIds, @event.Context.GroupId, @event.Context.RetailerCode);

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
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        public async Task WriteUpdateCommissionLogAsync(UpdatedCommissionIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    var content = await RenderUpdateCommissionAuditlog(@event.Commission, @event.BranchIds, @event.Context.GroupId, @event.Context.RetailerCode);
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
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        public async Task WriteDeleteCommissionLogAsync(DeletedCommissionIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    var content = $"Xóa bảng hoa hồng: {@event.Commission.Name}";
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
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        private async Task<string> RenderCreateCommissionAuditlog(Commission commission, List<int> branchIds, int groupId, string retailerCode = "")
        {
            string scope;
            if (branchIds.Any() || branchIds.Count > 0)
            {
                var branches = await KiotVietApiClient.GetBranchByIds(branchIds, commission.TenantId, groupId, retailerCode);
                scope = string.Join(", ", branches.Select(x => x.Name).ToList());
            }
            else
            {
                scope = "Toàn hệ thống";
            }

            var status = commission.IsActive ? "Áp dụng" : "Ngừng hoạt động";
            return "Thêm mới bảng hoa hồng:" +
                   $"<br/>- Tên: " + commission.Name +
                   $"<br/>- Phạm vi áp dụng: {scope}" +
                   $"<br/>- Trạng thái: {status}";
        }

        private async Task<string> RenderUpdateCommissionAuditlog(Commission commission, List<int> branchIds, int groupId, string retailerCode = "")
        {
            string scope;
            if (branchIds.Any() || branchIds.Count > 0)
            {
                var branches = await KiotVietApiClient.GetBranchByIds(branchIds, commission.TenantId, groupId, retailerCode);
                scope = string.Join(", ", branches.Select(x => x.Name).ToList());
            }
            else
            {
                scope = "Toàn hệ thống";
            }

            var status = commission.IsActive ? "Áp dụng" : "Ngừng hoạt động";
            return "Cập nhật bảng hoa hồng:" +
                   $"<br/>- Tên: " + commission.Name +
                   $"<br/>- Phạm vi áp dụng: {scope}" +
                   $"<br/>- Trạng thái: {status}";
        }
    }
}
