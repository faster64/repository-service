using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.SettingEvents;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using ServiceStack;
using static KiotVietTimeSheet.Domain.Utilities.Utilities;

namespace KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types
{
    public class SettingAuditProcess : BaseAuditProcess
    {
        public SettingAuditProcess(
            IKiotVietInternalService kiotVietInternalService
        ) : base(kiotVietInternalService)
        { }

        public async Task WriteUpdateSettingClockingGpsLogAsync(UpdateSettingClockingGpsIntegrationEvent @event)
        {
            var settingContentAuditLogs = new StringBuilder($"Cập nhật thông tin thiết lập chấm công: </br> ");
            settingContentAuditLogs.Append($"- Thiết lập chấm công trên thiết bị di dộng: {GetNameFromIsAuto(!@event.UseClockingGps)} -> {GetNameFromIsAuto(@event.UseClockingGps)}");

            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.GeneralSettings,
                TimeSheetAuditTrailAction.Update,
                settingContentAuditLogs.ToString(),
                @event.Context
            );

            await AddLogAsync(auditLog);
        }

        public async Task WriteUpdateSettingLogAsync(UpdatedSettingIntegrationEvent @event)
        {
            var oldSetting = @event.OldSetting;
            var newSetting = @event.NewSetting;
            List<string> settingContentAuditLogs;
            // thiết lập chấm công
            if (@event.SettingType == (byte)SettingType.Clocking)
            {
                var (isAutoCalcLateTime,
                isAutoCalcEarlyTime,
                isAutoCalcEarlyTimeOt,
                isAutoCalcLateTimeOt,
                isAutoTimekeepingMultiple,
                defaultTimeSetting,
                isAllowAutoKeeping)
                = CheckAutoCal(oldSetting, newSetting);

                settingContentAuditLogs =
                new List<string>
                {
                            $"Cập nhật thông tin {TimeSheetSettingsTypes.Timekeeping.ToDescription()}</br>",
                            $"{GetNameAuditString(defaultTimeSetting, "- Giờ mặc định khi chấm công vào, ra: {0}</br>")}",
                            $"{GetNameAuditString(isAllowAutoKeeping, "- Tự động chấm công theo ca làm việc: {0}</br>")}",
                            $"{GetNameAuditString(isAutoCalcLateTime, "- Tự động tính đi muộn: {0}</br>")}",
                            $"{GetNameAuditString(isAutoCalcEarlyTime, "- Tự động tính về sớm: {0}</br>")}",
                            $"{GetNameAuditString(isAutoCalcEarlyTimeOt, "- Tự động tính làm thêm giờ trước ca: {0}</br>")}",
                            $"{GetNameAuditString(isAutoCalcLateTimeOt, "- Tự động tính làm thêm giờ sau ca: {0}</br>")}",
                            $"{GetNameAuditString(isAutoTimekeepingMultiple, "- Tự động ghi nhận thời gian chấm công cho ca thỏa điều kiện và các ca trước đó: {0}</br>")}",
                };
            }
            // thiết lập tính lương
            else if(@event.SettingType == (byte)SettingType.TimeSheet)
            {
                var isAutoCreatePaySheet = RenderAutoCreatePaySheet(oldSetting, newSetting);

                var (startDateOfEveryMonth, startAndEndDateTwiceAMonthEqually) = GetStringStartDateOfEveryMonth(oldSetting, newSetting);

                var (startDayOfWeekEveryWeek, startDayOfWeekTwiceWeekly) = GetStartDayOfWeekEveryWeek(oldSetting, newSetting);

                var standardWorkingDay =
                    oldSetting.StandardWorkingDay != newSetting.StandardWorkingDay
                        ? $"{oldSetting.StandardWorkingDay} giờ -> {newSetting.StandardWorkingDay} giờ"
                        : string.Empty;

                var isChangeDaySalary =
                    !string.IsNullOrEmpty(startDateOfEveryMonth) ||
                    !string.IsNullOrEmpty(startAndEndDateTwiceAMonthEqually) ||
                    !string.IsNullOrEmpty(startDayOfWeekEveryWeek) ||
                    !string.IsNullOrEmpty(startDayOfWeekTwiceWeekly);

                var (halfShiftIsActive, halfShiftTime) = CheckHalfShiftIsActive(oldSetting, newSetting);
                settingContentAuditLogs =
                new List<string>
                {
                            $"Cập nhật thông tin {TimeSheetSettingsTypes.Paysheet.ToDescription()}:</br>",
                            $"{GetNameAuditChangeDaySalary(isChangeDaySalary,"Ngày bắt đầu tính lương:</br>")}",
                            $"{GetNameAuditString(startDateOfEveryMonth, "- Chi trả hàng tháng: {0}</br>")}",
                            $"{GetNameAuditString(startAndEndDateTwiceAMonthEqually,"- Chi trả 2 lần 1 tháng vào: {0}</br>")}",
                            $"{GetNameAuditString(startDayOfWeekEveryWeek,"- Chi trả hàng tuần vào: {0}</br>")}",
                            $"{GetNameAuditString(startDayOfWeekTwiceWeekly,"- Chi trả 2 tuần 1 lần vào: {0}</br>")}",
                            $"{GetNameAuditString(standardWorkingDay,"- Số giờ của 1 ngày công chuẩn: {0}</br>")}",
                            $"{GetNameAuditString(halfShiftIsActive,"- Tính nửa công cho nhân viên hưởng lương theo ngày công chuẩn : {0}</br>")}",
                            $"{GetNameAuditString(halfShiftTime,"&nbsp;&nbsp;&nbsp;Nếu trong một ngày công, nhân viên làm việc <= {0}. </br>")}",
                            $"{GetNameAuditString(isAutoCreatePaySheet, "- Tự động tạo bảng tính lương: {0}</br>")}",
                };
            }
            // thiết lập hoa hồng
            else
            {
                var template = string.Empty;
                if (oldSetting.CommissionSetting != newSetting.CommissionSetting)
                {
                    var getAllMsg = $"Nhận toàn bộ doanh thu dịch vụ";
                    var divideEqually = $"Chia đều doanh thu dịch vụ";
                    if (newSetting.CommissionSetting == (int)CommissionSettingType.DivideEqually)
                    {
                        template = $"{getAllMsg} -> {divideEqually}";
                    }
                    else
                    {
                        template = $"{divideEqually} -> {getAllMsg}";
                    }
                }
                settingContentAuditLogs =
                new List<string>
                {
                            $"Cập nhật thông tin {TimeSheetSettingsTypes.Commission.ToDescription()}:</br>",
                            $"{template}"
                };
            }

            var settingAuditLog = string.Join("", settingContentAuditLogs);

            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.GeneralSettings,
                TimeSheetAuditTrailAction.Update,
                settingAuditLog,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        #region Private Methods
        private Tuple<string, string> GetStringStartDateOfEveryMonth(SettingsToObject oldSetting, SettingsDto newSetting)
        {
            var startDateOfEveryMonth = string.Empty;
            var startAndEndDateTwiceAMonthEqually = string.Empty;
            var split = string.Empty;
            var yes = $"<strong>Có</strong>";
            var no = $"<strong>Không</strong>";

            if (oldSetting.IsDateOfEveryMonth != newSetting.IsDateOfEveryMonth)
            {
                startDateOfEveryMonth += newSetting.IsDateOfEveryMonth ? $"{no} -> {yes}" : $"{yes} -> {no}";
                split = ", ";
            }

            if (oldSetting.StartDateOfEveryMonth != newSetting.StartDateOfEveryMonth)
            {
                startDateOfEveryMonth += split + $"Ngày {oldSetting.StartDateOfEveryMonth} -> Ngày {newSetting.StartDateOfEveryMonth}";
            }

            if (oldSetting.IsDateOfTwiceAMonth != newSetting.IsDateOfTwiceAMonth)
            {
                startAndEndDateTwiceAMonthEqually = newSetting.IsDateOfTwiceAMonth ? $"{no} -> {yes}" : $"{yes} -> {no}";
                split = ", ";
            }

            if (oldSetting.FirstStartDateOfTwiceAMonth != newSetting.FirstStartDateOfTwiceAMonth ||
                oldSetting.SecondStartDateOfTwiceAMonth != newSetting.SecondStartDateOfTwiceAMonth)
            {
                startAndEndDateTwiceAMonthEqually += split +
                    $"Ngày {oldSetting.FirstStartDateOfTwiceAMonth} và Ngày {oldSetting.SecondStartDateOfTwiceAMonth} -> Ngày {newSetting.FirstStartDateOfTwiceAMonth} và Ngày {newSetting.SecondStartDateOfTwiceAMonth}";
            }

            return new Tuple<string, string>(startDateOfEveryMonth, startAndEndDateTwiceAMonthEqually);
        }

        private string GetNameAuditString(string message, string content)
        {
            return (string.IsNullOrEmpty(message) ? string.Empty : string.Format(content, message));
        }

        private string GetNameAuditChangeDaySalary(bool isChangeDaySalary, string content)
        {
            return isChangeDaySalary ? content : string.Empty;
        }

        private Tuple<string, string, string, string, string, string, string> CheckAutoCal(SettingsToObject oldSetting, SettingsDto newSetting)
        {
            var isAutoCalcLateTime = RenderIsAutoCalc(oldSetting.IsAutoCalcLateTime,
                newSetting.IsAutoCalcLateTime, false, oldSetting.LateTime, newSetting.LateTime);

            var isAutoCalcEarlyTime = RenderIsAutoCalc(oldSetting.IsAutoCalcEarlyTime,
                newSetting.IsAutoCalcEarlyTime, true, oldSetting.EarlyTime, newSetting.EarlyTime);

            var isAutoCalcEarlyTimeOt = RenderIsAutoCalc(oldSetting.IsAutoCalcEarlyTimeOT,
                newSetting.IsAutoCalcEarlyTimeOT, true, oldSetting.EarlyTimeOT, newSetting.EarlyTimeOT);

            var isAutoCalcLateTimeOt = RenderIsAutoCalc(oldSetting.IsAutoCalcLateTimeOT,
                newSetting.IsAutoCalcLateTimeOT, false, oldSetting.LateTimeOT, newSetting.LateTimeOT);

            var isAutoTimekeepingMultiple = RenderIsAutoTimekeepingMultiple(oldSetting, newSetting);

            var defaultTimeSetting = RenderDefaultTimeSetting(oldSetting, newSetting);

            var isAllowAutoKeeping = RenderAllowAutoKeeping(oldSetting, newSetting);

            return new Tuple<string, string, string, string, string, string, string>(isAutoCalcLateTime, isAutoCalcEarlyTime, isAutoCalcEarlyTimeOt, isAutoCalcLateTimeOt, isAutoTimekeepingMultiple, defaultTimeSetting, isAllowAutoKeeping);
        }

        private Tuple<string, string> GetStartDayOfWeekEveryWeek(SettingsToObject oldSetting, SettingsDto newSetting)
        {
            var startDayOfWeekEveryWeek = string.Empty;
            var startDayOfWeekTwiceWeekly = string.Empty;
            var split = string.Empty;
            var yes = $"<strong>Có</strong>";
            var no = $"<strong>Không</strong>";

            if (oldSetting.IsDayOfWeekEveryWeek != newSetting.IsDayOfWeekEveryWeek)
            {
                startDayOfWeekEveryWeek += newSetting.IsDayOfWeekEveryWeek ? $"{no} -> {yes}" : $"{yes} -> {no}";
                split = ", ";
            }

            if (oldSetting.StartDayOfWeekEveryWeek != newSetting.StartDayOfWeekEveryWeek)
            {
                startDayOfWeekEveryWeek += split +
                    $"{GetDayOfWeek(oldSetting.StartDayOfWeekEveryWeek)} -> {GetDayOfWeek(newSetting.StartDayOfWeekEveryWeek)}";
            }

            if (oldSetting.IsDayOfWeekTwiceWeekly != newSetting.IsDayOfWeekTwiceWeekly)
            {
                startDayOfWeekTwiceWeekly += newSetting.IsDayOfWeekTwiceWeekly ? $"{no} -> {yes}" : $"{yes} -> {no}";
                split = ", ";
            }

            if (oldSetting.StartDayOfWeekTwiceWeekly != newSetting.StartDayOfWeekTwiceWeekly)
            {
                startDayOfWeekTwiceWeekly += split +
                    $"{GetDayOfWeek(oldSetting.StartDayOfWeekTwiceWeekly)} -> {GetDayOfWeek(newSetting.StartDayOfWeekTwiceWeekly)}";
            }

            return new Tuple<string, string>(startDayOfWeekEveryWeek, startDayOfWeekTwiceWeekly);
        }

        private string RenderIsAutoTimekeepingMultiple(SettingsToObject oldSetting, SettingsDto newSetting)
        {
            var template = string.Empty;

            if (oldSetting.IsAutoTimekeepingMultiple != newSetting.IsAutoTimekeepingMultiple)
            {
                if (newSetting.IsAutoTimekeepingMultiple)
                {
                    template += $"<strong>Không</strong> -> <strong>Có</strong>";
                }
                else
                {
                    template += $"<strong>Có</strong> -> <strong>Không</strong>";
                }
            }

            if (oldSetting.MaxShiftIsAutoTimekeepingMultiple != newSetting.MaxShiftIsAutoTimekeepingMultiple)
            {
                template +=
                    $"Ghi nhận cho tối đa {oldSetting.MaxShiftIsAutoTimekeepingMultiple} ca -> {newSetting.MaxShiftIsAutoTimekeepingMultiple} ca";
            }

            if (oldSetting.RangeShiftIsAutoTimekeepingMultipleHours != newSetting.RangeShiftIsAutoTimekeepingMultipleHours ||
                oldSetting.RangeShiftIsAutoTimekeepingMultipleMinutes != newSetting.RangeShiftIsAutoTimekeepingMultipleMinutes)
            {
                template +=
                    $", mỗi ca cách nhau tối đa {oldSetting.RangeShiftIsAutoTimekeepingMultipleHours} giờ {oldSetting.RangeShiftIsAutoTimekeepingMultipleMinutes} phút -> {newSetting.RangeShiftIsAutoTimekeepingMultipleHours} giờ {newSetting.RangeShiftIsAutoTimekeepingMultipleMinutes}";
            }

            return template;
        }

        private string RenderDefaultTimeSetting(SettingsToObject oldSetting, SettingsDto newSetting)
        {
            var template = string.Empty;
            var shiftTime = $"<strong>Theo giờ bắt đầu, kết thúc ca</strong>";
            var clockingTime = $"<strong>Theo giờ thực hiện chấm công</strong>";

            if (oldSetting.DefaultTimeSetting != newSetting.DefaultTimeSetting)
            {
                template = newSetting.DefaultTimeSetting == 1 ? $"{shiftTime} -> {clockingTime}" : $"{clockingTime} -> {shiftTime}";
            }

            return template;
        }

        private string RenderAllowAutoKeeping(SettingsToObject oldSetting, SettingsDto newSetting)
        {
            var template = string.Empty;
            var yes = $"<strong>Có</strong>";
            var no = $"<strong>Không</strong>";

            if (oldSetting.AllowAutoKeeping != newSetting.AllowAutoKeeping)
            {
                template = newSetting.AllowAutoKeeping ? $"{no} -> {yes}" : $"{yes} -> {no}";
            }

            return template;
        }

        private string RenderAutoCreatePaySheet(SettingsToObject oldSetting, SettingsDto newSetting)
        {
            var template = string.Empty;
            var yes = $"<strong>Có</strong>";
            var no = $"<strong>Không</strong>";

            if (oldSetting.IsAutoCreatePaySheet != newSetting.IsAutoCreatePaySheet)
            {
                template = newSetting.IsAutoCreatePaySheet ? $"{no} -> {yes}" : $"{yes} -> {no}";
            }

            return template;
        }

        private string RenderIsAutoCalc(bool oldIsAuto, bool newIsAuto, bool isBefore, int oldTime, int newTime)
        {
            var nameBefore = isBefore ? "trước" : "sau";

            var nameNewIsAuto = oldTime != newTime && newIsAuto
                ? $"Tính {nameBefore} {oldTime} phút -> {newTime} phút"
                : string.Empty;

            var nameNewDifferentOld = !string.IsNullOrEmpty(nameNewIsAuto) ? ", " + nameNewIsAuto : nameNewIsAuto;

            return newIsAuto != oldIsAuto
                ? $"{GetNameFromIsAuto(oldIsAuto)} -> {GetNameFromIsAuto(newIsAuto)} {nameNewDifferentOld}"
                : $"{nameNewIsAuto}";
        }

        private Tuple<string, string> CheckHalfShiftIsActive(SettingsToObject oldSetting, SettingsDto newSetting)
        {
            var halfShiftIsActive = string.Empty;
            var template = string.Empty;

            if (oldSetting.HalfShiftIsActive != newSetting.HalfShiftIsActive)
            {
                if (newSetting.HalfShiftIsActive)
                {
                    halfShiftIsActive = $"<strong>Không</strong> -> <strong>Có</strong>";
                }
                else
                {
                    halfShiftIsActive = $"<strong>Có</strong> -> <strong>Không</strong>";
                }
            }

            if (newSetting.HalfShiftIsActive && oldSetting.HalfShiftIsActive != newSetting.HalfShiftIsActive)
            {
                template +=
                    $"{newSetting.HalfShiftMaxHour} giờ {newSetting.HalfShiftMaxMinute} phút";
            }

            if (newSetting.HalfShiftIsActive && oldSetting.HalfShiftIsActive == newSetting.HalfShiftIsActive && (newSetting.HalfShiftMaxHour != oldSetting.HalfShiftMaxHour || newSetting.HalfShiftMaxMinute != oldSetting.HalfShiftMaxMinute))
            {
                template +=
                    $"{oldSetting.HalfShiftMaxHour} giờ {oldSetting.HalfShiftMaxMinute} phút -> {newSetting.HalfShiftMaxHour} giờ {newSetting.HalfShiftMaxMinute} phút";
            }

            return new Tuple<string, string>(halfShiftIsActive, template);
        }

        private string GetNameFromIsAuto(bool isAuto)
        {
            return isAuto ? $"<strong>Có</strong>" : $"<strong>Không</strong>";
        }

        #endregion
    }
}
