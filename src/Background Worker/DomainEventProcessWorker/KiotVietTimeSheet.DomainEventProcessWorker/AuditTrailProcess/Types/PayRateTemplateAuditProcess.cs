using System;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.PayRateTemplateEvents;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.Infrastructure.AuditTrail;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;

namespace KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types
{
    public class PayRateTemplateAuditProcess : BaseAuditProcess
    {
        public PayRateTemplateAuditProcess(
            EfDbContext db, 
            IKiotVietApiClient kiotVietApiClient, 
            IAuditProcessFailEventService auditProcessFailEventService
            ) : base(kiotVietApiClient, auditProcessFailEventService)
        { }

        public async Task WriteCreatePayRateTemplateLogAsync(CreatedPayRateTemplateIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    var auditLog = GenerateLog(
                        TimeSheetFunctionTypes.EmployeeManagement,
                        TimeSheetAuditTrailAction.Create,
                        $"Thêm mới mẫu áp dụng: {@event.PayRateTemplate.Name}",
                        @event.Context
                    );
                    await AddLogAsync(auditLog, @event.Context.GroupId, @event.Context.RetailerCode);
                }
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        public async Task WriteUpdatePayRateTemplateLogAsync(UpdatedPayRateTemplateIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    var content = @event.UpdatePayRate
                        ? "Cập nhật mẫu áp dụng: " +
                          $"{RenderPayRateTemplateName(@event.OldPayRateTemplate.Name, @event.NewPayRateTemplate.Name)} " +
                          $"</br>Có cập nhật lại nhân viên đã có mẫu áp dụng {@event.OldPayRateTemplate.Name}"
                        : "Cập nhật mẫu áp dụng: " +
                          $"{RenderPayRateTemplateName(@event.OldPayRateTemplate.Name, @event.NewPayRateTemplate.Name)} " +
                          $"</br>Không cập nhật lại nhân viên đã có mẫu áp dụng {@event.OldPayRateTemplate.Name}";
                    var auditLog = GenerateLog(
                        TimeSheetFunctionTypes.EmployeeManagement,
                        TimeSheetAuditTrailAction.Update,
                        content,
                        @event.Context
                    );
                    await AddLogAsync(auditLog, @event.Context.GroupId, @event.Context.RetailerCode);
                }
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        public async Task WriteDeletePayRateTemplateLogAsync(DeletedPayRateTemplateIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    var auditLog = GenerateLog(
                        TimeSheetFunctionTypes.EmployeeManagement,
                        TimeSheetAuditTrailAction.Delete,
                        $"Xóa mẫu áp dụng: {@event.PayRateTemplate.Name}",
                        @event.Context
                    );
                    await AddLogAsync(auditLog, @event.Context.GroupId, @event.Context.RetailerCode);
                }
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        private static string RenderPayRateTemplateName(string oldPayRateTemplateName, string newPayRateTemplateName)
        {
            return oldPayRateTemplateName == newPayRateTemplateName ? $"{oldPayRateTemplateName}" : $"{oldPayRateTemplateName} -> {newPayRateTemplateName}";
        }
    }
}
