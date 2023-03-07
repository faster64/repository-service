using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.PenalizeEvents;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Common;

namespace KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types
{
    public class PenalizeAuditProcess : BaseAuditProcess
    {
        public PenalizeAuditProcess(IKiotVietInternalService kiotVietInternalService) : base(kiotVietInternalService)
        {
        }

        public async Task CreateLogAsync(CreatedPenalizeIntegrationEvent @event)
        {
            var content = $"Thêm mới vi phạm: : {@event.Penalize.Name}";
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.TimeSheetManagement,
                TimeSheetAuditTrailAction.Create,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task UpdateLogAsync(UpdatedPenalizeIntegrationEvent @event)
        {
            var oldPenalize = @event.PenalizeOld;
            var newPenalize = @event.PenalizeNew;
            var content = $"Cập nhật tên vi phạm: {oldPenalize.Name} -> {newPenalize.Name}";
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.TimeSheetManagement,
                TimeSheetAuditTrailAction.Update,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task DeleteLogAsync(DeletedPenalizeIntegrationEvent @event)
        {
            var content = $"Xoá vi phạm: {@event.Penalize.Name}";

            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.TimeSheetManagement,
                TimeSheetAuditTrailAction.Delete,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        #region Private Methods

        #endregion
    }
}
