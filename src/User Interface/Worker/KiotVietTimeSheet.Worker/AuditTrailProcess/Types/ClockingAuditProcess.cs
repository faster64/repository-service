using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.AutoKeepingEvents;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Queries.GetEmployeeById;
using KiotVietTimeSheet.Application.Queries.GetEmployeeByIds;
using KiotVietTimeSheet.Application.Queries.GetPenalizesByTenantId;
using KiotVietTimeSheet.Application.Queries.GetShiftById;
using KiotVietTimeSheet.Application.Queries.GetShiftByIds;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Auth;
using KiotVietTimeSheet.Utilities;
using KiotVietTimeSheet.Utilities.Const;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types
{
    public class ClockingAuditProcess : BaseAuditProcess
    {
        #region Injection

        private readonly IMediator _mediator;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly ILogger<ClockingAuditProcess> _logger;
        #endregion Injection

        public ClockingAuditProcess(
            ExecutionContext context,
            IKiotVietInternalService kiotVietInternalService,
            IMediator mediator,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            ILogger<ClockingAuditProcess> logger
        ) : base(context, kiotVietInternalService)
        {
            _mediator = mediator;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _logger = logger;
        }

        public async Task WriteSwapClockingLogAsync(SwappedClockingIntegrationEvent @event)
        {
            var oldClocking = @event.OldClocking;
            var newClocking = @event.NewClocking;

            var oldEmployee = await _mediator.Send(new GetEmployeeByIdQuery(oldClocking.EmployeeId));
            var oldEmployeeName = oldEmployee != null ? oldEmployee.Name : string.Empty;

            var newEmployee = await _mediator.Send(new GetEmployeeByIdQuery(newClocking.EmployeeId));
            var newEmployeeName = newEmployee != null ? newEmployee.Name : string.Empty;

            var oldShift = await _mediator.Send(new GetShiftByIdQuery(oldClocking.ShiftId));
            var oldShiftName = oldShift != null ? oldShift.Name : string.Empty;

            var newShift = await _mediator.Send(new GetShiftByIdQuery(newClocking.ShiftId));
            var newShiftName = newShift != null ? newShift.Name : string.Empty;

            var content = $"Đổi Chi tiết làm việc: {oldEmployeeName} - {oldShiftName} - Ngày {oldClocking.StartTime:dd/MM/yyyy} cho Chi tiết làm việc: {newEmployeeName} - {newShiftName} - Ngày {newClocking.StartTime:dd/MM/yyyy}";
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.TimeSheetManagement,
                TimeSheetAuditTrailAction.SwapClocking,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task WriteCreateClockingMultipleLogAsync(CreateMultipleClockingIntegrationEvent @event)
        {
            if (@event.FromAuto)
            {
                if (@event.TimeSheet != null)
                {
                    @event.Context.SetUserId(@event.TimeSheet.ModifiedBy ?? @event.TimeSheet.CreatedBy, "SystemAuto");
                    @event.Context.SetBranchId(@event.TimeSheet.BranchId);
                }
                if (@event.TimeSheetDto != null)
                {
                    @event.TimeSheetDto.StartDate = DateTime.Now;
                }
            }
            var clockingAttendances = @event.ListClockings;
            var tenantId = clockingAttendances.First()?.TenantId ?? 0;

            var clockingBranchIds = clockingAttendances.Select(x => x.BranchId).Distinct().ToList();
            var listBranch = await KiotVietInternalService.GetBranchByIdsAsync(clockingBranchIds, tenantId);
            var listShifts = await GetListShiftByClockingIdsAsync(clockingAttendances);
            var listEmployees = await GetEmployeesAsync(clockingAttendances);

            var clockingAuditContents =
                (from cl in clockingAttendances
                 from br in listBranch
                 from sh in listShifts
                 from em in listEmployees
                 orderby em.Name
                 where cl.BranchId == br.Id
                           && cl.ShiftId == sh.Id
                           && cl.EmployeeId == em.Id
                 select $"</br>- {em.Name} - {sh.Name} - {Label.date} {cl.StartTime:dd/MM/yyyy} - {br.Name}, {Label.status}: {RenderClockingStatusText(cl)}"
                ).ToList();

            var title = await GetCreateAuditTitle(@event);
            var content = $"{title}{Message.addWorkSchedule}: {string.Join("", clockingAuditContents)}";
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.TimeSheetManagement,
                TimeSheetAuditTrailAction.Create,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task WriteRejectClockingMultipleLogAsync(RejectMultipleClockingIntegrationEvent @event)
        {
            var clockAttendances = @event.ListClockings;
            var tenantId = clockAttendances.First()?.TenantId ?? 0;

            var clockingBranchIds = clockAttendances.Select(x => x.BranchId).Distinct().ToList();
            var listBranch = await KiotVietInternalService.GetBranchByIdsAsync(clockingBranchIds, tenantId);
            var listShifts = await GetListShiftByClockingIdsAsync(clockAttendances);
            var listEmployees = await GetEmployeesAsync(clockAttendances);

            var clockingAuditContention =
                (from cl in clockAttendances
                 from br in listBranch
                 from sh in listShifts
                 from em in listEmployees
                 orderby em.Name
                 where cl.BranchId == br.Id
                           && cl.ShiftId == sh.Id
                           && cl.EmployeeId == em.Id
                 select $"</br>- {em.Name} - {sh.Name} - {Label.date} {cl.StartTime:dd/MM/yyyy} - {br.Name}, {Label.status}: {RenderClockingStatusText(cl)}"
                ).ToList();

            var title = await GetRejectAuditTitle(@event);
            var content = $"{title}{Message.rejectWorkSchedule}: {string.Join("", clockingAuditContention)}";
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.TimeSheetManagement,
                TimeSheetAuditTrailAction.Reject,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task WriteUpdateClockingMultipleLogAsync(UpdateMultipleClockingIntegrationEvent @event)
        {
            var clockingAttendances = @event.ListClockings;
            var tenantId = clockingAttendances.First().Item2?.TenantId ?? 0;

            var clockingBranchIds = clockingAttendances.Select(x => x.Item2.BranchId).Distinct().ToList();
            var listBranch = await KiotVietInternalService.GetBranchByIdsAsync(clockingBranchIds, tenantId);

            var clockingAuditContents = clockingAttendances
                .OrderBy(c => c.Item1.EmployeeId)
                .Select(async c => await RenderUpdateClockingAuditAsync(c.Item1, c.Item2, listBranch))
                .Select(t => t.Result);
            var content = $"Cập nhật thông tin chi tiết làm việc: {string.Join("", clockingAuditContents)}";

            var stringBuildClockingPenalize = "- Phạt vi phạm";
            var listNewClockingsOld = clockingAttendances.Select(x => x.Item1).Where(x => x.ClockingPenalizes?.Any() == true).ToList();
            var stringBuildClockingPenalizeOld = await GenLogClockingPenalize(listNewClockingsOld.SelectMany(x => x.ClockingPenalizes.Where(y => y != null)).ToList());

            var listNewClockings = clockingAttendances.Select(x => x.Item2).Where(x => x.ClockingPenalizes?.Any() == true).ToList();
            var stringBuildClockingPenalizeNew = await GenLogClockingPenalize(listNewClockings.SelectMany(x => x.ClockingPenalizes.Where(y => y != null)).ToList());

            if (!string.IsNullOrEmpty(stringBuildClockingPenalizeOld) || !string.IsNullOrEmpty(stringBuildClockingPenalizeNew))
            {
                if (string.IsNullOrEmpty(stringBuildClockingPenalizeOld))
                {
                    content += $"<br /> {stringBuildClockingPenalize}: {stringBuildClockingPenalizeNew}";
                }
                else
                {
                    content += $"<br /> {stringBuildClockingPenalize}: {stringBuildClockingPenalizeOld} => {stringBuildClockingPenalizeNew}";
                }
            }

            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.TimeSheetManagement,
                TimeSheetAuditTrailAction.Update,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task WriteChangeClockingLogAsync(ChangedClockingIntegrationEvent @event)
        {
            var content = await GetChangeAuditTitle(@event);
            var auditLog = GenerateLog(
                TimeSheetFunctionTypes.TimeSheetManagement,
                TimeSheetAuditTrailAction.Update,
                content,
                @event.Context
            );
            await AddLogAsync(auditLog);
        }

        public async Task WriteAutoKeepingLogAsync(UpdateAutoKeepingIntegrationEvent @event)
        {
            try
            {
                _logger.LogInformation($"Handling WriteAutoKeepingLogAsync event tenantId:{@event.Context.TenantId}");

                var clockingAttendances = new List<Tuple<Clocking, Clocking>>();
                foreach (var clocking in @event.ListClockings)
                {
                    var newClocking = clocking.CreateCopy();
                    newClocking.UpdateClockingStatusAndAbsenceType((byte)ClockingStatuses.CheckedOut);
                    clockingAttendances.Add(new Tuple<Clocking, Clocking>(clocking, newClocking));
                }
                
                var tenantId = clockingAttendances.First().Item1?.TenantId ?? @event.Context.TenantId;
                var branchId = clockingAttendances.First().Item1?.BranchId ?? @event.Context.BranchId;
                var userId = clockingAttendances.First().Item1?.CreatedBy ?? @event.Context.UserId;
                var clockingBranchIds = clockingAttendances.Select(x => x.Item2.BranchId).Distinct().ToList();                
                var listBranch = await KiotVietInternalService.GetBranchByIdsAsync(clockingBranchIds, tenantId);
                var dctClockingAttendances = new Dictionary<long, List<Tuple<Clocking, Clocking>>>();
                var orderClockingAttendances = clockingAttendances
                                                .OrderBy(c => c.Item1.ShiftId);

                

                foreach (var item in orderClockingAttendances)
                {
                    var key = item.Item1.ShiftId;
                    if (dctClockingAttendances.ContainsKey(item.Item1.ShiftId))
                    {
                        dctClockingAttendances[key].Add(item);
                    }
                    else
                    {
                        var listITem = new List<Tuple<Clocking, Clocking>>();
                        listITem.Add(item);
                        dctClockingAttendances.Add(key, listITem);
                    }
                }
                var clockingAuditContents = "";
                var listShift = await _mediator.Send(new GetShiftByIdsQuery(dctClockingAttendances.Keys.ToList()));
                var dictShif= listShift.ToDictionary(x => x.Id, y => y.Name);
                foreach (var item in dctClockingAttendances)
                {                   
                    var oldShiftName = dictShif.ContainsKey(item.Key) ? dictShif[item.Key] : string.Empty;
                    var itemContent = "";
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        var it = item.Value[i];
                        itemContent += (await RenderAutoKeepingAuditAsync(it.Item1, it.Item2, listBranch, i==0));
                    }
                                       
                    clockingAuditContents += $" <br><ul style='list-style-type: disc;padding-left: 15px;'><li>{oldShiftName}</li></ul> {itemContent}";                        
                }
                
                var content = $"{Message.autoKeeping_log_title}: {string.Join("", clockingAuditContents)}";

                var auditLog = GenerateLog(
                    TimeSheetFunctionTypes.TimeSheetManagement,
                    TimeSheetAuditTrailAction.Update,
                    content,
                    @event.Context
                );
                auditLog.BranchId = branchId;
                auditLog.UserId = userId;
                @event.Context.SetBranchId(branchId);
                @event.Context.SetUserId(userId);
                await AddLogAsync(auditLog);
        }
            catch (Exception epx)
            {                
                _logger.LogError($"WriteAutoKeepingLogAsync error :{epx.Message}", epx);
            }

}

        #region Private Methods

        private async Task<string> GenLogClockingPenalize(List<ClockingPenalize> clockingPenalizes)
        {
            if (clockingPenalizes == null || !clockingPenalizes.Any())
            {
                return string.Empty;
            }

            var listPenalizeId = clockingPenalizes.GroupBy(x => x.PenalizeId).Select(x => x.Key).ToList();
            var penalizes = await _mediator.Send(new GetPenalizesByIdsQuery(listPenalizeId));
            var stringBuildClockingPenalize = "";
            clockingPenalizes.ForEach(item =>
            {
                var penalize = penalizes.FirstOrDefault(x => x.Id == item.PenalizeId);
                if (penalize == null)
                    return;

                stringBuildClockingPenalize += $" {penalize.Name}: {item.TimesCount}*{item.Value:n0},";
            });

            if (string.IsNullOrEmpty(stringBuildClockingPenalize))
            {
                return string.Empty;
            }

            stringBuildClockingPenalize = stringBuildClockingPenalize.Remove(stringBuildClockingPenalize.Length - 1);

            return stringBuildClockingPenalize;
        }

        private static string RenderClockingStatusText(Clocking clocking)
        {
            switch (clocking.ClockingStatus)
            {
                case (byte)ClockingStatuses.Void:
                    return ClockingStatusesExtension.Void.ToDescription();

                case (byte)ClockingStatuses.Created:
                    return ClockingStatusesExtension.Created.ToDescription();

                case (byte)ClockingStatuses.CheckedIn:
                    return ClockingStatusesExtension.CheckInNoCheckOut.ToDescription();

                case (byte)ClockingStatuses.CheckedOut:
                    return clocking.CheckInDate == null
                        ? ClockingStatusesExtension.CheckOutNoCheckIn.ToDescription()
                        : ClockingStatusesExtension.CheckInCheckOut.ToDescription();

                case (byte)ClockingStatuses.WorkOff:
                    return clocking.AbsenceType == (byte)AbsenceTypes.AuthorisedAbsence
                        ? "Nghỉ có phép"
                        : "Nghỉ không phép";

                default: return string.Empty;
            }
        }

        private async Task<List<EmployeeDto>> GetEmployeesAsync(List<Clocking> listClocking)
        {
            var employeeIds = listClocking.GroupBy(x => x.EmployeeId).Select(x => x.Key).ToList();
            return await _mediator.Send(new GetEmployeeByIdsQuery(employeeIds));
        }

        private async Task<List<ShiftDto>> GetListShiftByClockingIdsAsync(List<Clocking> listClocking)
        {
            var shiftIds = listClocking.Select(x => x.ShiftId).Distinct().ToList();
            return await _mediator.Send(new GetShiftByIdsQuery(shiftIds));
        }

        private async Task<string> RenderUpdateClockingAuditAsync(Clocking oldClocking, Clocking newClocking, List<BranchDto> listBranch)
        {
            var employee = await _mediator.Send(new GetEmployeeByIdQuery(oldClocking.EmployeeId));
            var employeeName = employee?.Name ?? string.Empty;

            var oldShift = await _mediator.Send(new GetShiftByIdQuery(oldClocking.ShiftId));
            var oldShiftName = oldShift?.Name ?? string.Empty;

            var newShift = await _mediator.Send(new GetShiftByIdQuery(newClocking.ShiftId));
            var newShiftName = newShift?.Name ?? string.Empty;

            var branchName = listBranch.FirstOrDefault(x => x.Id == newClocking.BranchId)?.Name;
            var shiftChange = string.IsNullOrEmpty(newShiftName) || newShiftName == oldShiftName ? string.Empty : $", {oldShiftName} => {newShiftName}";
            var dateChange = DateTime.Compare(oldClocking.StartTime, newClocking.StartTime) == 0 ? string.Empty : $", Ngày {oldClocking.StartTime:dd/MM/yyyy} => {newClocking.StartTime:dd/MM/yyyy}";

            // KCCTL-239 Check trường hợp mà clocking ở trạng thái đã vào đã ra => đã vào chưa ra
            var statusChange = string.Empty;
            if (oldClocking.ClockingStatus == newClocking.ClockingStatus)
            {
                if (oldClocking.CheckInDate == null && newClocking.CheckInDate != null ||
                    oldClocking.CheckInDate != null && newClocking.CheckInDate == null ||
                    oldClocking.CheckOutDate == null && newClocking.CheckOutDate != null ||
                    oldClocking.CheckOutDate != null && newClocking.CheckOutDate == null)
                {
                    statusChange = $", Trạng thái: {RenderClockingStatusText(oldClocking)} => {RenderClockingStatusText(newClocking)}";
                }
            }
            else
            {
                statusChange = oldClocking.ClockingStatus == newClocking.ClockingStatus && oldClocking.AbsenceType == newClocking.AbsenceType ? string.Empty : $", Trạng thái: {RenderClockingStatusText(oldClocking)} => {RenderClockingStatusText(newClocking)}";
            }

            var content = $"</br>{employeeName} - {oldShiftName} - Ngày {oldClocking.StartTime:dd/MM/yyyy} - {branchName}{shiftChange}{dateChange}{statusChange}";
            return content;
        }

        private async Task<string> RenderAutoKeepingAuditAsync(Clocking oldClocking, Clocking newClocking, List<BranchDto> listBranch, bool isFirst)
        {
            var employee = await _mediator.Send(new GetEmployeeByIdQuery(oldClocking.EmployeeId));
            var employeeName = employee?.Name ?? string.Empty;
            var branchName = listBranch.FirstOrDefault(x => x.Id == newClocking.BranchId)?.Name;            
            var statusChange = string.Empty;
            statusChange = $", {Message.autoKeeping_log_detail_status_title}: {RenderClockingStatusText(oldClocking)} => Đã vào - Đã ra";
            var content = isFirst ? "": "</br>";            
            content += $"{employeeName} - {Message.autoKeeping_log_detail_date} {oldClocking.StartTime:dd/MM/yyyy} - {branchName}{statusChange}";
            return content;
        }

        private async Task<(string, string, string, bool?, bool?)> GetCreateAuditTitleOld(TimeSheet timeSheetOld, string language, bool hasChange = false, bool forAllClockings = false)
        {
            var title = string.Empty;
            var change = string.Empty;
            var changeRepeat = string.Empty;
            bool? hasEndDate = null;
            bool? saveOnHoliday = null;
            if (hasChange && timeSheetOld != null)
            {
                saveOnHoliday = timeSheetOld.SaveOnHoliday;
                hasEndDate = timeSheetOld.AutoGenerateClockingStatus != (byte)AutoGenerateClockingStatus.Auto;
                change += string.Format(Message.changeFrom, forAllClockings ? Message.forAllWork : Message.forCurrentTo);
                var emp = await _employeeReadOnlyRepository.FindByIdAsync(timeSheetOld.EmployeeId);
                if (emp != null)
                {
                    title += string.Format(Message.editEmployeeClocking, emp.Name);
                }
                changeRepeat = await GetCreateAuditRepeat(timeSheetOld.IsRepeat, timeSheetOld.RepeatType,
                    timeSheetOld.RepeatEachDay, timeSheetOld.TimeSheetShifts, language);
            }
            return (title, change, changeRepeat, hasEndDate, saveOnHoliday);
        }

        private async Task<string> GetCreateAuditRepeat(bool? isRepeat, byte? repeatType, byte? repeatEachDay, IReadOnlyCollection<TimeSheetShift> timeSheetShifts, string language)
        {
            var changeRepeat = new StringBuilder();
            if (isRepeat.GetValueOrDefault())
            {
                if (repeatType is (byte)RepeatTypes.Daily)
                {
                    var shiftIds = timeSheetShifts?.Select(x => x.ShiftIds).Join(",");
                    changeRepeat.Append(string.Format(Message.auditShiftRepeat, $"{repeatEachDay} {Label.date.ToLower()}", await GetShiftName(shiftIds)));
                }
                else if (repeatType is (byte)RepeatTypes.Weekly && timeSheetShifts != null)
                {
                    foreach (var tsShift in timeSheetShifts)
                    {
                        changeRepeat.Append(string.Format(Message.auditShiftRepeat,
                            $"{repeatEachDay} {Label.Weekly.ToLower()} ({KvConst.GetDayNameList(language ?? "vi-VN", tsShift.RepeatDaysOfWeek)})",
                            await GetShiftName(tsShift.ShiftIds)));
                    }
                }
            }
            return changeRepeat.ToString();
        }

        private async Task<string> GetCreateAuditTitle(CreateMultipleClockingIntegrationEvent @event)
        {
            var timeSheet = @event.TimeSheetDto;
            if (timeSheet == null) return string.Empty;

            var (title, change, changeRepeat, hasEndDate, saveOnHoliday) = await GetCreateAuditTitleOld(@event.TimeSheet, @event.Context?.Language, @event.HasChange, @event.ForAllClockings);
            if (timeSheet.IsRepeat.GetValueOrDefault())
            {
                title += GetCreateAuditHeadTitle(@event, hasEndDate);
                var repeat = await GetCreateAuditRepeat(timeSheet.IsRepeat, timeSheet.RepeatType,
                    timeSheet.RepeatEachDay, timeSheet.TimeSheetShifts.ConvertTo<List<TimeSheetShift>>(),
                    @event.Context?.Language ?? "vi-VN");
                if (repeat != changeRepeat && !string.IsNullOrEmpty(changeRepeat))
                {
                    title += $"{changeRepeat} =><br>{repeat}";
                    repeat = string.Empty;
                }
                title += repeat;
                var soho = saveOnHoliday.GetValueOrDefault() ? Label.yes : Label.no;
                var soh = timeSheet.SaveOnHoliday ? Label.yes : Label.no;
                title += string.Format(Message.createOnHoliday,
                    saveOnHoliday != null && saveOnHoliday != timeSheet.SaveOnHoliday
                        ? $"{soho} => {soh}"
                        : soh);
                title += change;
            }
            return title;
        }

        private static string GetCreateAuditHeadTitle(CreateMultipleClockingIntegrationEvent @event, bool? hasEndDate)
        {
            var timeSheet = @event.TimeSheetDto;
            var hedo = hasEndDate.GetValueOrDefault() ? Label.no : Label.yes;
            var hed = timeSheet.HasEndDate ? Label.no : Label.yes;
            var mode = @event.IsManual ? Label.manual.ToLower() : Label.auto.ToLower();
            if (@event.FromAuto) mode = $"{Label.system.ToLower()} {Label.auto.ToLower()}";
            return string.Format(Message.auditCreateTitle,
                mode,
                hasEndDate != null && hasEndDate != timeSheet.HasEndDate ? $"{hedo} => {hed}" : hed,
                timeSheet.StartDate.ToShortDateString(),
                timeSheet.HasEndDate
                    ? string.Format(Message.endDate, timeSheet.EndDate.ToShortDateString())
                    : string.Empty);
        }

        private async Task<string> GetRejectAuditTitle(RejectMultipleClockingIntegrationEvent @event)
        {
            var timeSheet = @event.TimeSheet;
            if (timeSheet == null) return string.Empty;

            var (title, _, _, hasEndDate, _) = await GetCreateAuditTitleOld(@event.TimeSheet, @event.Context?.Language, @event.HasChange);
            title += string.Format(Message.auditRejectHeadTitle, timeSheet.IsRepeat.GetValueOrDefault() ? Label.auto.ToLower() : Label.manual.ToLower());
            if (timeSheet.IsRepeat.GetValueOrDefault())
            {
                if (@event.HasChange)
                {
                    title += string.Format(Message.auditCreateShortTitle,
                        hasEndDate.GetValueOrDefault() ? Label.no : Label.yes,
                        timeSheet.StartDate.ToShortDateString(),
                        hasEndDate.GetValueOrDefault() ? string.Format(Message.endDate, timeSheet.EndDate.ToShortDateString()) : string.Empty);
                }
                else
                {
                    title += string.Format(Message.auditRejectTitle,
                        timeSheet.StartDate.ToShortDateString(),
                        string.Format(Message.endDate, timeSheet.EndDate.ToShortDateString()));
                }
            }
            return title;
        }

        private async Task<string> GetShiftName(string shiftIds)
        {
            if (string.IsNullOrEmpty(shiftIds)) return string.Empty;
            var ids = shiftIds.Split(',')?.ToObjects().ToArray();
            var shifts = ids != null ? await _shiftReadOnlyRepository.FindByIdsAsync(ids) : null;
            return shifts == null ? string.Empty : shifts.Select(x => x.Name).Join(" - ");
        }

        private async Task<string> GetChangeAuditTitle(ChangedClockingIntegrationEvent @event)
        {
            var timeSheet = @event.TimeSheetDto;
            if (timeSheet == null) return string.Empty;

            var (title, change, changeRepeat, hasEndDate, saveOnHoliday) = await GetCreateAuditTitleOld(@event.TimeSheet, @event.Context?.Language, @event.HasChange, @event.ForAllClockings);
            var hedo = hasEndDate.GetValueOrDefault() ? Label.no : Label.yes;
            var hed = timeSheet.HasEndDate ? Label.no : Label.yes;
            title += string.Format(Message.auditCreateShortTitle,
                hasEndDate != null && hasEndDate != timeSheet.HasEndDate ? $"{hedo} => {hed}" : hed,
                timeSheet.StartDate.ToShortDateString(),
                string.Format(Message.endDate, timeSheet.EndDate.ToShortDateString()));
            var repeat = await GetCreateAuditRepeat(timeSheet.IsRepeat, timeSheet.RepeatType,
                timeSheet.RepeatEachDay, timeSheet.TimeSheetShifts.ConvertTo<List<TimeSheetShift>>(),
                @event.Context?.Language ?? "vi-VN");
            if (repeat != changeRepeat && !string.IsNullOrEmpty(changeRepeat))
            {
                title += $"{changeRepeat} =><br>{repeat}";
                repeat = string.Empty;
            }
            title += repeat;
            var soho = saveOnHoliday.GetValueOrDefault() ? Label.yes : Label.no;
            var soh = timeSheet.SaveOnHoliday ? Label.yes : Label.no;
            title += string.Format(Message.createOnHoliday,
                saveOnHoliday != null && saveOnHoliday != timeSheet.SaveOnHoliday
                    ? $"{soho} => {soh}"
                    : soh);
            title += change;
            return title;
        }

        #endregion Private Methods
    }
}