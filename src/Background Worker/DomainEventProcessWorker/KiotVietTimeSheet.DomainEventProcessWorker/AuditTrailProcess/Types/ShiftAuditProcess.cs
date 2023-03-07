using System;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.ShiftEvents;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.Infrastructure.AuditTrail;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;

namespace KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types
{
    public class ShiftAuditProcess : BaseAuditProcess
    {
        public ShiftAuditProcess(
            EfDbContext db, 
            IKiotVietApiClient kiotVietApiClient, 
            IAuditProcessFailEventService auditProcessFailEventService
        ) : base(kiotVietApiClient, auditProcessFailEventService)
        { }

        public async Task WriteCreateShiftLogAsync(CreatedShiftIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    var auditLog = GenerateLog(
                        TimeSheetFunctionTypes.ShiftManagement,
                        TimeSheetAuditTrailAction.Create,
                        $"Thêm mới ca làm việc: {@event.Shift.Name}</br>" +
                        $" Thời gian cho phép nhân viên chấm công: " +
                        $"{RenderLessCheckIn(@event.Shift.CheckInBefore)}" +
                        $" - " +
                        $"{RenderThanCheckOut(@event.Shift.CheckOutAfter)}",
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

        public async Task WriteUpdateShiftLogAsync(UpdatedShiftIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    var content = $"Cập nhật thông tin ca làm việc: " +
                                  $"{RenderShiftName(@event.OldShift.Name, @event.Shift.Name)}" +
                                  $"{RenderShiftFrom(@event.OldShift.From, @event.Shift.From)}" +
                                  $"{RenderShiftTo(@event.OldShift.To, @event.Shift.To)}" +
                                  $"</br> Thời gian cho phép nhân viên chấm công: " +
                                  $"{RenderLessCheckIn(@event.Shift.CheckInBefore)}" +
                                  $" - " +
                                  $"{RenderThanCheckOut(@event.Shift.CheckOutAfter)} </br>" +
                                  $"{RenderShiftStatus(@event.OldShift.IsActive, @event.Shift.IsActive)}";

                    content = content.Substring(0, content.Length - 2);
                    var auditLog = GenerateLog(
                        TimeSheetFunctionTypes.ShiftManagement,
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

        public async Task WriteDeleteShiftLogAsync(DeletedShiftIntegrationEvent @event)
        {
            try
            {
                var content = $"Xóa ca làm việc: {@event.Shift.Name}";
                var auditLog = GenerateLog(
                    TimeSheetFunctionTypes.ShiftManagement,
                    TimeSheetAuditTrailAction.Delete,
                    content,
                    @event.Context
                );
                await AddLogAsync(auditLog, @event.Context.GroupId, @event.Context.RetailerCode);
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        #region Private Methods
        private static string RenderShiftName(string oldShiftName, string newShiftName)
        {
            return oldShiftName == newShiftName ? $"{oldShiftName}, " : $"{oldShiftName} -> {newShiftName}, ";
        }

        private static string RenderShiftFrom(long oldShiftFrom, long newShiftFrom)
        {
            if (oldShiftFrom == newShiftFrom)
            {
                return string.Empty;
            }
            var dateTime = DateTime.Now.Date;
            return $"Giờ làm từ {dateTime.AddMinutes(oldShiftFrom):HH:mm} -> {dateTime.AddMinutes(newShiftFrom):HH:mm}, ";
        }

        private static string RenderShiftTo(long oldShiftTo, long newShiftTo)
        {
            if (oldShiftTo == newShiftTo)
            {
                return string.Empty;
            }
            var dateTime = DateTime.Now.Date;
            return $"Giờ làm đến {dateTime.AddMinutes(oldShiftTo):HH:mm} -> {dateTime.AddMinutes(newShiftTo):HH:mm}, ";
        }

        private static string RenderShiftStatus(bool oldShiftStatus, bool newShiftStatus)
        {
            return oldShiftStatus == newShiftStatus ? string.Empty : $"{RenderStatus(oldShiftStatus)} -> {RenderStatus(newShiftStatus)}, ";
        }

        private static string RenderStatus(bool status)
        {
            return status ? "Hoạt động" : "Ngừng hoạt động";
        }

        private static string RenderLessCheckIn(long? checkInBefore)
        {
            var dateTime = DateTime.Now.Date;
            if (checkInBefore == null) return string.Empty;
            return $"Chấm vào sau: {dateTime.AddMinutes(checkInBefore.Value):HH:mm}";
        }

        private static string RenderThanCheckOut(long? checkOutAfter)
        {
            var dateTime = DateTime.Now.Date;
            if (checkOutAfter == null) return string.Empty;
            return $"Chấm ra trước: {dateTime.AddMinutes(checkOutAfter.Value):HH:mm}";
        }
        #endregion
    }
}
