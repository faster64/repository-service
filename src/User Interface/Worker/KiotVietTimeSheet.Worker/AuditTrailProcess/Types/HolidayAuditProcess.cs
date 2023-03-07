using System;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.HolidayEvents;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Common;
namespace KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types
{
    public class HolidayAuditProcess : BaseAuditProcess
    {
        public HolidayAuditProcess(
            IKiotVietInternalService kiotVietInternalService
        ) : base(kiotVietInternalService)
        { }

        public async Task WriteCreateHolidayLog(CreatedHolidayIntegrationEvent @event)
        {
            var content = $"Thêm mới kỳ lễ tết: {@event.Holiday.Name}";
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.GeneralSettings,
                TimeSheetAuditTrailAction.Create,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task WriteUpdateHolidayLog(UpdatedHolidayIntegrationEvent @event)
        {
            var oldHoliday = @event.OldHoliday;
            var newHoliday = @event.NewHoliday;
            var content = $"Cập nhật thông tin kỳ lễ tết: " +
                          $"{RenderHolidayName(oldHoliday.Name, newHoliday.Name)}" +
                          $"{RenderHolidayFrom(oldHoliday.From, newHoliday.From)}" +
                          $"{RenderHolidayTo(oldHoliday.To, newHoliday.To)}";

            content = content.Substring(0, content.Length - 2);
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.GeneralSettings,
                TimeSheetAuditTrailAction.Update,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task WriteDeleteHolidayLog(DeletedHolidayIntegrationEvent @event)
        {
            var content = $"Xóa kỳ lễ tết: {@event.Holiday.Name}";
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.GeneralSettings,
                TimeSheetAuditTrailAction.Delete,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        #region Private Methods
        private static string RenderHolidayName(string oldHolidayName, string newHolidayName)
        {
            return oldHolidayName == newHolidayName ? $"{oldHolidayName} ," : $"{oldHolidayName} -> {newHolidayName}, ";
        }

        private static string RenderHolidayFrom(DateTime oldHolidayFrom, DateTime newHolidayFrom)
        {
            return DateTime.Compare(oldHolidayFrom.Date, newHolidayFrom.Date) == 0 ? string.Empty : $"Từ ngày {oldHolidayFrom:dd/MM/yyyy} -> {newHolidayFrom:dd/MM/yyyy}, ";
        }

        private static string RenderHolidayTo(DateTime oldHolidayTo, DateTime newHolidayTo)
        {
            return DateTime.Compare(oldHolidayTo.Date, newHolidayTo.Date) == 0 ? string.Empty : $"Đến ngày {oldHolidayTo:dd/MM/yyyy} -> {newHolidayTo:dd/MM/yyyy}, ";
        }

        #endregion
    }
}
