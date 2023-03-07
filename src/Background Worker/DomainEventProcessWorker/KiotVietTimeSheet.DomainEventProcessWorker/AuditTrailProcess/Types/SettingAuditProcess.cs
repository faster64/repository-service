using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.SettingEvents;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.Infrastructure.AuditTrail;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using ServiceStack;
using static KiotVietTimeSheet.Domain.Utilities.Utilities;

namespace KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types
{
    public class SettingAuditProcess : BaseAuditProcess
    {
        public SettingAuditProcess(
            IKiotVietApiClient kiotVietApiClient,
            IAuditProcessFailEventService auditProcessFailEventService
        ) : base(kiotVietApiClient, auditProcessFailEventService)
        {
        }

        public async Task WriteUpdateSettingLogAsync(UpdatedSettingIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    var oldSetting = @event.OldSetting;
                    var newSetting = @event.NewSetting;

                    var (isAutoCalcLateTime, 
                        isAutoCalcEarlyTime, 
                        isAutoCalcEarlyTimeOt, 
                        isAutoCalcLateTimeOt,
                        isAutoTimekeepingMultiple) = CheckAutoCal(oldSetting, newSetting);

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

                    var settingContentAuditLogs =
                        new List<string>
                        {
                            $"Cập nhật thông tin thiết lập tính năng: {TimeSheetFunctionTypes.EmployeeManagement.ToDescription()}</br>",
                            $"{GetNameAuditString(isAutoCalcLateTime, "- Tự động tính đi muộn: {0}</br>")}",
                            $"{GetNameAuditString(isAutoCalcEarlyTime, "- Tự động tính về sớm: {0}</br>")}",
                            $"{GetNameAuditString(isAutoCalcEarlyTimeOt, "- Tự động tính làm thêm giờ trước ca: {0}</br>")}",
                            $"{GetNameAuditString(isAutoCalcLateTimeOt, "- Tự động tính làm thêm giờ sau ca: {0}</br>")}",
                            $"{GetNameAuditString(isAutoTimekeepingMultiple, "- Tự động ghi nhận thời gian chấm công cho ca thỏa điều kiện và các ca trước đó: {0}</br>")}",
                            $"{GetNameAuditChangeDaySalary(isChangeDaySalary,"- Ngày bắt đầu tính lương")}</br>",
                            $"{GetNameAuditString(startDateOfEveryMonth, "&nbsp;&nbsp;&nbsp;&nbsp;Hàng tháng: {0}</br>")}",
                            $"{GetNameAuditString(startAndEndDateTwiceAMonthEqually,"&nbsp;&nbsp;&nbsp;&nbsp;2 lần 1 tháng: {0}</br>")}",
                            $"{GetNameAuditString(startDayOfWeekEveryWeek,"&nbsp;&nbsp;&nbsp;&nbsp;Hàng tuần: {0}</br>")}",
                            $"{GetNameAuditString(startDayOfWeekTwiceWeekly,"&nbsp;&nbsp;&nbsp;&nbsp;2 tuần 1 lần: {0}</br>")}",
                            $"{GetNameAuditString(standardWorkingDay,"- 1 ngày công chuẩn: {0}</br>")}",
                        };

                    var settingAuditLog = string.Join("", settingContentAuditLogs);

                    var auditLog = GenerateLog(
                        TimeSheetFunctionTypes.EmployeeManagement,
                        TimeSheetAuditTrailAction.Update,
                        settingAuditLog,
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

        private Tuple<string, string> GetStringStartDateOfEveryMonth(SettingsToObject oldSetting, SettingsDto newSetting)
        {
            var startDateOfEveryMonth = oldSetting.StartDateOfEveryMonth != newSetting.StartDateOfEveryMonth
                ? $"Ngày {oldSetting.StartDateOfEveryMonth} -> Ngày {newSetting.StartDateOfEveryMonth}"
                : string.Empty;

            var startAndEndDateTwiceAMonthEqually = string.Empty;
            if (oldSetting.FirstStartDateOfTwiceAMonth != newSetting.FirstStartDateOfTwiceAMonth ||
                oldSetting.SecondStartDateOfTwiceAMonth != newSetting.SecondStartDateOfTwiceAMonth)
            {
                startAndEndDateTwiceAMonthEqually =
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

        private Tuple<string, string, string, string, string> CheckAutoCal(SettingsToObject oldSetting, SettingsDto newSetting)
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

            return new Tuple<string, string, string, string, string>(isAutoCalcLateTime, isAutoCalcEarlyTime, isAutoCalcEarlyTimeOt, isAutoCalcLateTimeOt, isAutoTimekeepingMultiple);
        }

        private Tuple<string, string> GetStartDayOfWeekEveryWeek(SettingsToObject oldSetting, SettingsDto newSetting)
        {
            var startDayOfWeekEveryWeek = string.Empty;
            if (oldSetting.StartDayOfWeekEveryWeek != newSetting.StartDayOfWeekEveryWeek)
            {
                startDayOfWeekEveryWeek =
                    $"{GetDayOfWeek(oldSetting.StartDayOfWeekEveryWeek)} -> {GetDayOfWeek(newSetting.StartDayOfWeekEveryWeek)}";
            }

            var startDayOfWeekTwiceWeekly = string.Empty;
            if (oldSetting.StartDayOfWeekTwiceWeekly != newSetting.StartDayOfWeekTwiceWeekly)
            {
                startDayOfWeekTwiceWeekly =
                    $"{GetDayOfWeek(oldSetting.StartDayOfWeekTwiceWeekly)} -> {GetDayOfWeek(newSetting.StartDayOfWeekTwiceWeekly)}";
            }

            return new Tuple<string, string>(startDayOfWeekEveryWeek, startDayOfWeekTwiceWeekly);
        }

        #region Private Methods
        private string RenderIsAutoTimekeepingMultiple(SettingsToObject oldSetting, SettingsDto newSetting)
        {
            var template = string.Empty;
            
            var nameIsAutoTimekeepingMultiple = newSetting.IsAutoTimekeepingMultiple ? "Có" : "Không";

            template += oldSetting.IsAutoTimekeepingMultiple != newSetting.IsAutoTimekeepingMultiple
                ? nameIsAutoTimekeepingMultiple
                : string.Empty;

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

        private string GetNameFromIsAuto(bool isAuto)
        {
            return isAuto ? "Có" : "Không";
        }

        #endregion
    }
}
