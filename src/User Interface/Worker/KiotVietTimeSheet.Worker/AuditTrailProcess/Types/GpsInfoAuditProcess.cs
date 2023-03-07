using KiotVietTimeSheet.Application.EventBus.Events.GpsInfoEvents;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Common;
using Microsoft.Extensions.Logging;
using ServiceStack;
using System;
using System.Text;
using KiotVietTimeSheet.Resources;
using System.Threading.Tasks;
using Message = KiotVietTimeSheet.Resources.Message;

namespace KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types
{
    public class GpsInfoAuditProcess : BaseAuditProcess
    {
        private readonly ILogger<GpsInfoAuditProcess> _logger;
        public GpsInfoAuditProcess(
        IKiotVietInternalService kiotVietInternalService,
        ILogger<GpsInfoAuditProcess> logger
        ) : base(kiotVietInternalService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task WriteCreateGpsInfoAsync(CreatedGpsInfoIntegrationEvent @event)
        {
            string branchName = string.Empty;
            var createGpsInfoContentAuditLogs = new StringBuilder($"Cập nhật thông tin thiết lập chấm công: </br> ");
            createGpsInfoContentAuditLogs.Append($"Thiết lập chấm công trên thiết bị di dộng: <br>");
            if (@event.GpsInfo != null)
            {
                try
                {
                    var branch = await KiotVietInternalService.GetBranchByIdAsync(@event.GpsInfo.BranchId, @event.Context.TenantId);
                    if (branch != null) branchName = branch.Name;
                }
                catch (Exception epx)
                {
                    _logger.LogError(epx.Message, epx);
                }

                createGpsInfoContentAuditLogs.Append($"- Thêm xác nhận GPS cho {branchName} <br>");
                createGpsInfoContentAuditLogs.Append($"- Địa chỉ: {@event.GpsInfo.Address} <br>");
                createGpsInfoContentAuditLogs.Append($"- Khu vực: {@event.GpsInfo.Province} - {@event.GpsInfo.District} <br>");
                createGpsInfoContentAuditLogs.Append($"- Phường: {@event.GpsInfo.WardName} <br>");
                createGpsInfoContentAuditLogs.Append($"- Phạm vi: {@event.GpsInfo.RadiusLimit:#}m <br>");
            }
            else
            {
                createGpsInfoContentAuditLogs.Append($"- Thêm xác nhận GPS  <br>");
            }

            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.GeneralSettings,
                TimeSheetAuditTrailAction.Create,
                createGpsInfoContentAuditLogs.ToString(),
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task WriteDeleteGpsInfoAsync(DeletedGpsInfoIntegrationEvent @event)
        {
            string branchName = string.Empty;
            var createGpsInfoContentAuditLogs = new StringBuilder($"Cập nhật thông tin thiết lập chấm công: </br> ");
            createGpsInfoContentAuditLogs.Append($"Thiết lập chấm công trên thiết bị di dộng: <br>");
            if (@event.GpsInfo != null)
            {
                try
                {
                    var branch = await KiotVietInternalService.GetBranchByIdAsync(@event.GpsInfo.BranchId, @event.Context.TenantId);
                    if (branch != null) branchName = branch.Name;
                }
                catch (Exception epx)
                {
                    _logger.LogError(epx.Message, epx);
                }
                createGpsInfoContentAuditLogs.Append($"- Xoá xác nhận GPS của {branchName} <br>");
            }
            else
            {
                createGpsInfoContentAuditLogs.Append($"- Xoá xác nhận GPS <br>");
            }
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.GeneralSettings,
                TimeSheetAuditTrailAction.Delete,
                createGpsInfoContentAuditLogs.ToString(),
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task WriteUpdateGpsInfoAsync(UpdatedGpsInfoIntegrationEvent @event)
        {
            string branchName = string.Empty;
            var createGpsInfoContentAuditLogs = new StringBuilder($"Cập nhật thông tin thiết lập chấm công: </br> ");
            createGpsInfoContentAuditLogs.Append($"Thiết lập chấm công trên thiết bị di dộng: <br>");
            if (@event.GpsInfoNew != null)
            {
                try
                {
                    var branch = await KiotVietInternalService.GetBranchByIdAsync(@event.GpsInfoNew.BranchId, @event.Context.TenantId);
                    if (branch != null) branchName = branch.Name;
                }
                catch (Exception epx)
                {
                    _logger.LogError(epx.Message, epx);
                }
                createGpsInfoContentAuditLogs.Append($"- Chỉnh sửa xác nhận GPS cho {branchName} <br>");
                createGpsInfoContentAuditLogs.Append($"- Địa chỉ: {@event.GpsInfoOld?.Address} -> {@event.GpsInfoNew.Address} <br>");
                createGpsInfoContentAuditLogs.Append($"- Khu vực: {@event.GpsInfoOld.Province} - {@event.GpsInfoOld?.District} -> {@event.GpsInfoNew.Province} - {@event.GpsInfoNew.District} <br>");
                createGpsInfoContentAuditLogs.Append($"- Phường: {@event.GpsInfoOld?.WardName} -> {@event.GpsInfoNew.WardName} <br>");
                createGpsInfoContentAuditLogs.Append($"- Phạm vi: {@event.GpsInfoOld?.RadiusLimit} -> {@event.GpsInfoNew.RadiusLimit:#}m <br>");
            }
            else
            {
                createGpsInfoContentAuditLogs.Append($"- Chỉnh sửa xác nhận GPS <br>");
            }
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.GeneralSettings,
                TimeSheetAuditTrailAction.Update,
                createGpsInfoContentAuditLogs.ToString(),
                @event.Context
            );

            await AddLogAsync(auditLog);
        }

        public async Task WriteUpdateQrKeyAsync(UpdatedQrKeyIntegrationEvent @event)
        {
            var content = $"{Message.updateSettingInfo}: {Label.manageEmployees} <br>{Message.clockingSetting}: <br>- {string.Format(Message.changeQR, @event.BranchName)}: <strong>{Label.yes}</strong>";
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.PosParameter,
                TimeSheetAuditTrailAction.Update,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }
    }
}
