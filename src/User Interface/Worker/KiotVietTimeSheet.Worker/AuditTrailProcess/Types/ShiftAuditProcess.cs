using System;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.ShiftEvents;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Common;

namespace KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types
{
    public class ShiftAuditProcess : BaseAuditProcess
    {
        public ShiftAuditProcess(
            IKiotVietInternalService kiotVietInternalService
        ) : base(kiotVietInternalService)
        { }

        public async Task WriteCreateShiftLogAsync(CreatedShiftIntegrationEvent @event)
        {
            long.TryParse(@event.Shift.CheckInBefore.ToString(), out var checkIn);
            long.TryParse(@event.Shift.CheckOutAfter.ToString(), out var checkOut);
            long.TryParse(@event.Shift.From.ToString(), out var from);
            long.TryParse(@event.Shift.To.ToString(), out var to);
            TimeSheetFunctionTypes functionType = @event.IsGeneralSetting ? TimeSheetFunctionTypes.GeneralSettings : TimeSheetFunctionTypes.EmployeeManagement;
            var auditLog = GenerateLog(
                    functionType,
                    TimeSheetAuditTrailAction.Create,
                    $"Thêm mới Ca làm việc: {@event.Shift.Name}</br>" +
                    $"- Giờ làm từ: {DateTime.Now.Date.AddMinutes(from):HH:mm}</br>" +
                    $"- Giờ làm đến: {DateTime.Now.Date.AddMinutes(to):HH:mm}</br>" +
                    $"Thời gian cho phép nhân viên chấm công: </br>" +
                    $"- {RenderLessCheckIn(checkIn)}</br>" +
                    $"- {RenderThanCheckOut(checkOut)}",
                @event.Context
                );
            await AddLogAsync(auditLog);
        }

        public async Task WriteUpdateShiftLogAsync(UpdatedShiftIntegrationEvent @event)
        {
            long checkIn = 0;
            long checkOut = 0;
            long.TryParse(@event.Shift.CheckInBefore.ToString(), out checkIn);
            long.TryParse(@event.Shift.CheckOutAfter.ToString(), out checkOut);
            bool isChangeCheckinCheckout = (@event.OldShift.CheckInBefore != @event.Shift.CheckInBefore || @event.OldShift.CheckOutAfter != @event.Shift.CheckOutAfter);
            TimeSheetFunctionTypes functionType = @event.IsGeneralSetting ? TimeSheetFunctionTypes.GeneralSettings : TimeSheetFunctionTypes.EmployeeManagement;
            var content = $"Cập nhật thông tin Ca làm việc: " +
                                  $"{RenderShiftName(@event.OldShift.Name, @event.Shift.Name)}" +
                                  $"{RenderShiftFrom(@event.OldShift.From, @event.Shift.From)}" +
                                  $"{RenderShiftTo(@event.OldShift.To, @event.Shift.To)}" +
                                  $"{(isChangeCheckinCheckout ? "Thời gian cho phép nhân viên chấm công: </br>" : "")}" +
                                  $"{RenderCheckIn(@event.OldShift.CheckInBefore ?? 0, @event.Shift.CheckInBefore ?? 0)}" +
                                  $"{RenderCheckOut(@event.OldShift.CheckOutAfter ?? 0, @event.Shift.CheckOutAfter ?? 0)}" +
                                  $"{RenderShiftStatus(@event.OldShift.IsActive, @event.Shift.IsActive)}";

            content = content.Substring(0, content.Length - 2);
            var auditLog = GenerateLog(
                functionType,
                TimeSheetAuditTrailAction.Update,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task WriteDeleteShiftLogAsync(DeletedShiftIntegrationEvent @event)
        {
            var content = $"Xóa ca làm việc: {@event.Shift.Name}";
            TimeSheetFunctionTypes functionType = @event.IsGeneralSetting ? TimeSheetFunctionTypes.GeneralSettings : TimeSheetFunctionTypes.EmployeeManagement;
            var auditLog = GenerateLog(
                functionType,
                TimeSheetAuditTrailAction.Delete,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        #region Private Methods
        private static string RenderShiftName(string oldShiftName, string newShiftName)
        {
            return oldShiftName == newShiftName ? $"{oldShiftName}</br>" : $"{oldShiftName} -> {newShiftName}</br>";
        }

        private static string RenderShiftFrom(long oldShiftFrom, long newShiftFrom)
        {
            if (oldShiftFrom == newShiftFrom)
            {
                return string.Empty;
            }
            var dateTime = DateTime.Now.Date;
            return $"- Giờ làm từ: {dateTime.AddMinutes(oldShiftFrom):HH:mm} -> {dateTime.AddMinutes(newShiftFrom):HH:mm}</br>";
        }

        private static string RenderShiftTo(long oldShiftTo, long newShiftTo)
        {
            if (oldShiftTo == newShiftTo)
            {
                return string.Empty;
            }
            var dateTime = DateTime.Now.Date;
            return $"- Giờ làm đến: {dateTime.AddMinutes(oldShiftTo):HH:mm} -> {dateTime.AddMinutes(newShiftTo):HH:mm}</br>";
        }

        private static string RenderShiftStatus(bool oldShiftStatus, bool newShiftStatus)
        {
            return oldShiftStatus == newShiftStatus ? string.Empty : $"{RenderStatus(oldShiftStatus)} -> {RenderStatus(newShiftStatus)}, ";
        }

        private static string RenderStatus(bool status)
        {
            return status ? "Hoạt động" : "Ngừng hoạt động";
        }

        private static string RenderLessCheckIn(long checkInBefore)
        {
            var dateTime = DateTime.Now.Date;
            return $"Chấm vào sau: {dateTime.AddMinutes(checkInBefore):HH:mm}";
        }

        private static string RenderThanCheckOut(long checkOutAfter)
        {
            var dateTime = DateTime.Now.Date;
            return $"Chấm ra trước: {dateTime.AddMinutes(checkOutAfter):HH:mm}";
        }

        private static string RenderCheckIn(long oldCheckInBefore, long newCheckInBefore)
        {
            if (oldCheckInBefore == newCheckInBefore)
            {
                return string.Empty;
            }
            var dateTime = DateTime.Now.Date;
            return $"- Chấm vào sau: {dateTime.AddMinutes(oldCheckInBefore):HH:mm} -> {dateTime.AddMinutes(newCheckInBefore):HH:mm}</br>";
        }

        private static string RenderCheckOut(long oldCheckOutAfter, long newCheckOutAfter)
        {
            if (oldCheckOutAfter == newCheckOutAfter)
            {
                return string.Empty;
            }
            var dateTime = DateTime.Now.Date;
            return $"- Chấm ra trước: {dateTime.AddMinutes(oldCheckOutAfter):HH:mm} -> {dateTime.AddMinutes(newCheckOutAfter):HH:mm}</br>";
        }
        #endregion
    }
}
