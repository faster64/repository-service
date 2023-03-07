using System;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.HolidayEvents;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.Infrastructure.AuditTrail;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;

namespace KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types
{
    public class HolidayAuditProcess : BaseAuditProcess
    {
        public HolidayAuditProcess(
            IKiotVietApiClient kiotVietApiClient, 
            IAuditProcessFailEventService auditProcessFailEventService
        ) : base(kiotVietApiClient, auditProcessFailEventService)
        { }

        public async Task WriteCreateHolidayLog(CreatedHolidayIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    var content = $"Thêm mới kỳ lễ tết: {@event.Holiday.Name}";
                    var auditLog = GenerateLog(
                        TimeSheetFunctionTypes.HolidayManagement,
                        TimeSheetAuditTrailAction.Create,
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

        public async Task WriteUpdateHolidayLog(UpdatedHolidayIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    var oldHoliday = @event.OldHoliday;
                    var newHoliday = @event.NewHoliday;
                    var content = $"Cập nhật thông tin kỳ lễ tết: " +
                                  $"{RenderHolidayName(oldHoliday.Name, newHoliday.Name)}" +
                                  $"{RenderHolidayFrom(oldHoliday.From, newHoliday.From)}" +
                                  $"{RenderHolidayTo(oldHoliday.To, newHoliday.To)}";

                    content = content.Substring(0, content.Length - 2);
                    var auditLog = GenerateLog(
                        TimeSheetFunctionTypes.HolidayManagement,
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

        public async Task WriteDeleteHolidayLog(DeletedHolidayIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    var content = $"Xóa kỳ lễ tết: {@event.Holiday.Name}";
                    var auditLog = GenerateLog(
                        TimeSheetFunctionTypes.HolidayManagement,
                        TimeSheetAuditTrailAction.Delete,
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
