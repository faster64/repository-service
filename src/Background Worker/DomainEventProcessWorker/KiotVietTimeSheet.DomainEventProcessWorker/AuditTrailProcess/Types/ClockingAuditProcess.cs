using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.Infrastructure.AuditTrail;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient.Dtos;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using Microsoft.EntityFrameworkCore;
using ServiceStack;
using ServiceStack.Configuration;

namespace KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types
{
    public class ClockingAuditProcess : BaseAuditProcess
    {
        private readonly IAppSettings _appSettings;
        private EfDbContext _db;
        private readonly IKiotVietApiClient _kiotVietApiClient;
        private readonly ICacheClient _cacheClient;
        private readonly Helper _helper = new Helper();

        public ClockingAuditProcess(
            IKiotVietApiClient kiotVietApiClient,
            IAuditProcessFailEventService auditProcessFailEventService,
            IAppSettings appSettings, ICacheClient cacheClient
        ) : base(kiotVietApiClient, auditProcessFailEventService)
        {
            _kiotVietApiClient = kiotVietApiClient;
            _appSettings = appSettings;
            _cacheClient = cacheClient;
        }
        public async Task WriteSwapClockingLogAsync(SwappedClockingIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    using (_db = await _helper.GetDbContextByGroupId(@event.Context.GroupId, @event.Context.RetailerCode, _cacheClient, _kiotVietApiClient, _appSettings))
                    {
                        var oldClocking = @event.OldClocking;
                        var newClocking = @event.NewClocking;

                        var oldEmployee = await _db.Employees.FindAsync(oldClocking.EmployeeId);
                        var oldEmployeeName = oldEmployee != null ? oldEmployee.Name : string.Empty;

                        var newEmployee = await _db.Employees.FindAsync(newClocking.EmployeeId);
                        var newEmployeeName = newEmployee != null ? newEmployee.Name : string.Empty;

                        var oldShift = await _db.Shifts.FindAsync(oldClocking.ShiftId);
                        var oldShiftName = oldShift != null ? oldShift.Name : string.Empty;

                        var newShift = await _db.Shifts.FindAsync(newClocking.ShiftId);
                        var newShiftName = newShift != null ? newShift.Name : string.Empty;

                        var content = $"Đổi Chi tiết làm việc: {oldEmployeeName} - {oldShiftName} - Ngày {oldClocking.StartTime:dd/MM/yyyy} cho Chi tiết làm việc: {newEmployeeName} - {newShiftName} - Ngày {newClocking.StartTime:dd/MM/yyyy}";
                        var auditLog = GenerateLog(
                            TimeSheetFunctionTypes.TimeSheetManagement,
                            TimeSheetAuditTrailAction.SwapClocking,
                            content,
                            @event.Context
                        );
                        await AddLogAsync(auditLog, @event.Context.GroupId, @event.Context.RetailerCode);
                    }

                }
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        public async Task WriteCreateClockingMultipleLogAsync(CreateMultipleClockingIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    using (_db = await _helper.GetDbContextByGroupId(@event.Context.GroupId,
                        @event.Context.RetailerCode, _cacheClient, _kiotVietApiClient, _appSettings))
                    {
                        var clockingAttendances = @event.ListClockings;
                        var tenantId = clockingAttendances.First()?.TenantId ?? 0;

                        var clockingBranchIds = clockingAttendances.Select(x => x.BranchId).Distinct().ToList();
                        var listBranch = await KiotVietApiClient.GetBranchByIds(clockingBranchIds, tenantId, @event.Context.GroupId);
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
                             select $"</br>- {em.Name} - {sh.Name} - Ngày {cl.StartTime:dd/MM/yyyy} - {br.Name}, Trạng thái: {RenderClockingStatusText(cl)}"
                            ).ToList();

                        var content = $"Thêm mới chi tiết làm việc: {string.Join("", clockingAuditContents)}";
                        var auditLog = GenerateLog(
                            TimeSheetFunctionTypes.TimeSheetManagement,
                            TimeSheetAuditTrailAction.Create,
                            content,
                            @event.Context
                        );
                        await AddLogAsync(auditLog, @event.Context.GroupId, @event.Context.RetailerCode);
                    }


                }
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        public async Task WriteRejectClockingMultipleLogAsync(RejectMultipleClockingIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    using (_db = await _helper.GetDbContextByGroupId(@event.Context.GroupId,
                        @event.Context.RetailerCode, _cacheClient, _kiotVietApiClient, _appSettings))
                    {
                        var clockAttendances = @event.ListClockings;
                        var tenantId = clockAttendances.First()?.TenantId ?? 0;

                        var clockingBranchIds = clockAttendances.Select(x => x.BranchId).Distinct().ToList();
                        var listBranch = await KiotVietApiClient.GetBranchByIds(clockingBranchIds, tenantId, @event.Context.GroupId);
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
                             select $"</br>- {em.Name} - {sh.Name} - Ngày {cl.StartTime:dd/MM/yyyy} - {br.Name}, Trạng thái: {RenderClockingStatusText(cl)}"
                            ).ToList();

                        var content = $"Hủy chi tiết làm việc: {string.Join("", clockingAuditContention)}";

                        var auditLog = GenerateLog(
                            TimeSheetFunctionTypes.TimeSheetManagement,
                            TimeSheetAuditTrailAction.Reject,
                            content,
                            @event.Context
                        );
                        await AddLogAsync(auditLog, @event.Context.GroupId, @event.Context.RetailerCode);
                    }
                }
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }

        public async Task WriteUpdateClockingMultipleLogAsync(UpdateMultipleClockingIntegrationEvent @event)
        {
            try
            {
                if (@event != null)
                {
                    using (_db = await _helper.GetDbContextByGroupId(@event.Context.GroupId,
                        @event.Context.RetailerCode, _cacheClient, _kiotVietApiClient, _appSettings))
                    {
                        var clockingAttendances = @event.ListClockings;
                        var tenantId = clockingAttendances.First().Item2?.TenantId ?? 0;

                        var clockingBranchIds = clockingAttendances.Select(x => x.Item2.BranchId).Distinct().ToList();
                        var listBranch = await KiotVietApiClient.GetBranchByIds(clockingBranchIds, tenantId, @event.Context.GroupId);

                        var clockingAuditContents = clockingAttendances
                            .OrderBy(c => c.Item1.EmployeeId)
                            .Select(async c => await RenderUpdateClockingAuditAsync(c.Item1, c.Item2, listBranch))
                            .Select(t => t.Result);
                        var content = $"Cập nhật thông tin chi tiết làm việc: {string.Join("", clockingAuditContents)}";

                        var auditLog = GenerateLog(
                            TimeSheetFunctionTypes.TimeSheetManagement,
                            TimeSheetAuditTrailAction.Update,
                            content,
                            @event.Context
                        );
                        await AddLogAsync(auditLog, @event.Context.GroupId, @event.Context.RetailerCode);
                    }
                }
            }
            catch (Exception ex)
            {
                await Retry(@event, ex);
            }
        }
        #region Private Methods

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

        private async Task<List<Employee>> GetEmployeesAsync(List<Clocking> listClocking)
        {
            var employeeIds = listClocking.GroupBy(x => x.EmployeeId).Select(x => x.Key).ToList();
            return await _db.Employees.Where(x => employeeIds.Contains(x.Id)).ToListAsync();
        }

        private async Task<List<Shift>> GetListShiftByClockingIdsAsync(List<Clocking> listClocking)
        {
            var shiftIds = listClocking.Select(x => x.Id).ToList();
            return await _db.Shifts.Where(x => shiftIds.Contains(x.Id)).ToListAsync();
        }

        private async Task<string> RenderUpdateClockingAuditAsync(Clocking oldClocking, Clocking newClocking, List<Branch> listBranch)
        {
            var employee = await _db.Employees.FindAsync(oldClocking.EmployeeId);
            var employeeName = employee?.Name ?? string.Empty;

            var oldShift = await _db.Shifts.FindAsync(oldClocking.ShiftId);
            var oldShiftName = oldShift?.Name ?? string.Empty;

            var newShift = await _db.Shifts.FindAsync(newClocking.ShiftId);
            var newShiftName = newShift?.Name ?? string.Empty;

            var branchName = listBranch.FirstOrDefault(x => x.Id == newClocking.BranchId)?.Name;
            var shiftChange = string.IsNullOrEmpty(newShiftName) || newShiftName == oldShiftName ? string.Empty : $", {oldShiftName} -> {newShiftName}";
            var dateChange = DateTime.Compare(oldClocking.StartTime, newClocking.StartTime) == 0 ? string.Empty : $", Ngày {oldClocking.StartTime:dd/MM/yyyy} -> {newClocking.StartTime:dd/MM/yyyy}";

            // KCCTL-239 Check trường hợp mà clocking ở trạng thái đã vào đã ra -> đã vào chưa ra 
            var statusChange = string.Empty;
            if (oldClocking.ClockingStatus == newClocking.ClockingStatus)
            {
                if (oldClocking.CheckInDate == null && newClocking.CheckInDate != null ||
                    oldClocking.CheckInDate != null && newClocking.CheckInDate == null ||
                    oldClocking.CheckOutDate == null && newClocking.CheckOutDate != null ||
                    oldClocking.CheckOutDate != null && newClocking.CheckOutDate == null)
                {
                    statusChange = $", Trạng thái: {RenderClockingStatusText(oldClocking)} -> {RenderClockingStatusText(newClocking)}";
                }
            }
            else
            {
                statusChange = oldClocking.ClockingStatus == newClocking.ClockingStatus && oldClocking.AbsenceType == newClocking.AbsenceType ? string.Empty : $", Trạng thái: {RenderClockingStatusText(oldClocking)} -> {RenderClockingStatusText(newClocking)}";
            }

            var content = $"</br>- {employeeName} - {oldShiftName} - Ngày {oldClocking.StartTime:dd/MM/yyyy} - {branchName}{shiftChange}{dateChange}{statusChange}";
            return content;
        }
        #endregion
    }
}
