using KiotVietTimeSheet.Api.ServiceModel;
using MediatR;
using ServiceStack;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using Message = KiotVietTimeSheet.Resources.Message;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Application.Dto;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Queries.GetClockingsByTimeSheetId;
using KiotVietTimeSheet.Application.Queries.GetClockingsByShiftId;
using KiotVietTimeSheet.Application.Queries.GetClockingsForCalendar;
using KiotVietTimeSheet.Application.Queries.GetClockingsMultipleBranch;
using KiotVietTimeSheet.Application.Queries.GetClockingById;
using KiotVietTimeSheet.Application.Queries.GetClockingForSwap;
using KiotVietTimeSheet.Application.Queries.GetClockingForPaySlipClockingDetail;
using KiotVietTimeSheet.Application.Queries.GetClockingsByBranchId;
using KiotVietTimeSheet.Application.Commands.RejectClockingMultiple;
using KiotVietTimeSheet.Application.Commands.RejectClockingByFilter;
using KiotVietTimeSheet.Application.Commands.RejectClockingByBranches;
using KiotVietTimeSheet.Application.Commands.SwapClocking;
using KiotVietTimeSheet.Application.Commands.BatchUpdateClocking;
using KiotVietTimeSheet.Application.Commands.UpdateWhenUseAutomatedTimeKeeping;
using KiotVietTimeSheet.Application.Commands.BatchUpdateWhenUseAutomatedTimeKeeping;
using KiotVietTimeSheet.Application.Commands.UpdateClocking;
using KiotVietTimeSheet.Application.Commands.UpdateClockingMultipleBranchShiftAndDateTime;
using KiotVietTimeSheet.Application.Commands.GetBranchesWhenHaveAnyClocking;
using KiotVietTimeSheet.Application.Queries.GetShiftMultipleBranchOrderByFromAndTo;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class ClockingApi : BaseApi
    {
        #region Properties
        private readonly IMediator _mediator;
        public IAutoQueryDb AutoQuery { get; set; }
        #endregion

        #region Constructors
        public ClockingApi(
            ILogger<ClockingApi> logger,
            INotificationHandler<DomainNotification> notificationHandler,
            IMediator mediator

        ) : base(logger, notificationHandler)
        {
            _mediator = mediator;
        }
        #endregion

        #region GET methods

        /// <summary>
        /// Get clocking by shift id
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<object> Get(GetClockingsByShiftIdReq req)
        {
            var returnObj = await _mediator.Send(new GetClockingsByShiftIdQuery(req.ShiftId));
            return Ok(returnObj);
        }

        public async Task<object> Get(GetClockingsByTimeSheetIdReq req)
        {
            var returnObj = await _mediator.Send(new GetClockingsByTimeSheetIdQuery(req.TimeSheetId));
            return Ok(returnObj);
        }

        public async Task<object> Get(GetClockingForCalendarReq req)
        {
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            var result = await _mediator.Send(new GetClockingsForCalendarQuery(query, req.BranchId, req.ClockingHistoryStates, req.DepartmentIds));
            return Ok(result);
        }

        public async Task<object> Get(GetClockingMultipleBranchForCalendarReq req)
        {

            var pageSourceClocking = await _mediator.Send(new GetClockingsMultipleBranchForCalendarQuery(req.BranchIds, req.ClockingHistoryStates, req.DepartmentIds, req.ShiftIds, req.EmployeeIds, req.StartTime, req.EndTime, req.ClockingStatusExtension));
            if (pageSourceClocking?.Data == null || !pageSourceClocking.Data.Any())
            {
                return pageSourceClocking;
            }

            var shifts = await _mediator.Send(new GetShiftMultipleBranchOrderByFromAndToQuery(req.BranchIds, req.ShiftIds, null, false));

            switch (req.TypeCalendar)
            {
                case TypeCalendarView.KvTimeLineMonth:
                    var clockingMonths = new PagingDataSource<ClockingMonthDto>();
                    clockingMonths.Total = pageSourceClocking.Total;
                    clockingMonths.Filters = pageSourceClocking.Filters;
                    pageSourceClocking.Data = pageSourceClocking.Data.OrderBy(x => x.Employee.Name).ToList();
                    var clockingMonthDtos = new List<ClockingMonthDto>();
                    shifts.ForEach(shift =>
                    {
                        var clockingSource = (from entity in pageSourceClocking.Data
                                              where entity.ShiftId == shift.Id
                                              orderby entity.Employee?.Name.Trim().Split(' ').LastOrDefault()
                                              select new ClockingMonthDto
                                              {
                                                  Id = entity.Id.ToString(),
                                                  ResourceId = string.Join("_", entity.ShiftId, entity.WorkById),
                                                  EmployeeId = entity.EmployeeId,
                                                  WorkById = entity.WorkById,
                                                  ShiftId = entity.ShiftId,
                                                  Title = GetNameEmployee(entity.Employee?.Name),
                                                  Start = entity.StartTime,
                                                  End = entity.EndTime,
                                                  Clocking = entity
                                              }).ToList();

                        if (clockingSource.Any())
                        {
                            clockingMonthDtos.AddRange(clockingSource);
                        }
                    });
                    clockingMonths.Data = clockingMonthDtos;
                    return Ok(clockingMonths);

                case TypeCalendarView.KvTimeLineWeek:
                    var clockingWeeks = new PagingDataSource<ClockingWeekDto>();
                    clockingWeeks.Total = pageSourceClocking.Total;
                    clockingWeeks.Filters = pageSourceClocking.Filters;
                    var clockingWeekDtos = new List<ClockingWeekDto>();
                    shifts.ForEach(shift =>
                    {
                        var clockingSource = (from entity in pageSourceClocking.Data
                                              where entity.ShiftId == shift.Id
                                              orderby entity.Employee?.Name.Trim().Split(' ').LastOrDefault()
                                              select new ClockingWeekDto
                                              {
                                                  Id = entity.Id.ToString(),
                                                  ResourceId = entity.ShiftId.ToString(),
                                                  Title = entity.Employee?.Name ?? "Nhân viên bị xóa",
                                                  EmployeeId = entity.EmployeeId,
                                                  WorkById = entity.WorkById,
                                                  Start = entity.StartTime,
                                                  End = entity.EndTime,
                                                  Clocking = entity
                                              }).ToList();
                        if (clockingSource.Any())
                        {
                            clockingWeekDtos.AddRange(clockingSource);
                        }
                    });
                    clockingWeeks.Data = clockingWeekDtos;
                    return Ok(clockingWeeks);

                case TypeCalendarView.KvTimeLineDay:
                    var clockingDays = new PagingDataSource<ClockingDayDto>();
                    clockingDays.Total = pageSourceClocking.Total;
                    clockingDays.Filters = pageSourceClocking.Filters;

                    var clockingDayDtos = new List<ClockingDayDto>();
                    shifts.ForEach(shift =>
                    {
                        var clockingSource = (from entity in pageSourceClocking.Data
                                              where entity.ShiftId == shift.Id
                                              orderby entity.Employee?.Name.Trim().Split(' ').LastOrDefault()
                                              select new ClockingDayDto
                                              {
                                                  Id = entity.Id.ToString(),
                                                  ResourceId = entity.ShiftId.ToString(),
                                                  Title = entity.Employee?.Name ?? "Nhân viên bị xóa",
                                                  Start = entity.StartTime,
                                                  End = entity.EndTime,
                                                  Clocking = entity
                                              }).ToList();

                        if (clockingSource.Any())
                        {
                            clockingDayDtos.AddRange(clockingSource);
                        }
                    });
                    clockingDays.Data = clockingDayDtos;
                    return Ok(clockingDays);

                default:
                    //order to look like in view clockings
                    var clockingDtos = new List<ClockingDto>();
                    shifts.ForEach(shift =>
                    {
                        var clockingSource = (from entity in pageSourceClocking.Data
                                              where entity.ShiftId == shift.Id
                                              orderby entity.Employee?.Name
                                              select entity).ToList();

                        if (clockingSource.Any())
                        {
                            clockingDtos.AddRange(clockingSource);
                        }
                    });
                    pageSourceClocking.Data = clockingDtos;
                    return Ok(pageSourceClocking);
            }
        }

        public async Task<object> Get(GetClockingByIdReq req)
        {
            return Ok(await _mediator.Send(new GetClockingByIdQuery(req.Id)));
        }

        public async Task<object> Get(GetClockingForSwapReq req)
        {
            return Ok(await _mediator.Send(new GetClockingForSwapQuery(req.EmployeeId, req.Day, req.BranchId.GetValueOrDefault(), req.ShiftId)));

        }

        public async Task<object> Get(GetClockingForPaySlipClockingDetailReq req)
        {
            req.PaysheetStartTime = req.PaysheetStartTime.Date;
            req.PaysheetEndTime = req.PaysheetEndTime.AddDays(1).Date;
            var query = AutoQuery.CreateQuery(req, Request.GetRequestParams());
            var result = await _mediator.Send(new GetClockingForPaySlipClockingDetailQuery(query));
            return Ok(result);
        }

        public async Task<object> Get(GetClockingsByBranchIdReq req)
        {
            var returnObj = await _mediator.Send(new GetClockingsByBranchIdQuery(req.BranchId));
            return Ok(returnObj);
        }
        public async Task<object> Get(GetBranchesWithDeletePermissionHaveAnyClocking req)
        {
            var result = await _mediator.Send(new GetBranchesWhenHaveAnyClockingAndClockingDeletePermissionCommand(req.EmployeeId, req.BranchCancelIds));
            if (Errors.Any())
            {
                return BadRequest(Errors.FirstOrDefault()?.Message, Errors);
            }

            return Ok(result);
        }
        private string GetNameEmployee(string nameEmployee)
        {
            return string.IsNullOrEmpty(nameEmployee) ? "Nhân viên bị xóa" : " ";
        }
        #endregion

        #region POST methods
        public async Task<object> Post(RejectClockingByIdsReq req)
        {
            var returnObj = await _mediator.Send(new RejectClockingMultipleCommand(req.Ids));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(returnObj);
        }

        public async Task<object> Post(RejectClockingsByFilterReq req)
        {
            var returnObj = await _mediator.Send(new RejectClockingByFilterCommand(req.BranchId, req.EmployeeIds, req.StartDate, req.EndDate, req.ShiftId, req.StatusesExtension));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(returnObj);
        }
        public async Task<object> Post(RejectClockingsByBranchesReq req)
        {
            var returnObj = await _mediator.Send(new RejectClockingByBranchesCommand(req.BranchIds, req.EmployeeId, req.StartDate, req.EndDate, req.Statuses, req.IforAllClockings));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(returnObj);
        }

        public async Task<object> Post(SwapShiftClockingReq req)
        {
            var returnObj = await _mediator.Send(new SwapClockingCommand(req.ClockingId1, req.ClockingId2, req.EmployeeId1, req.EmployeeId2));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(returnObj);
        }

        public async Task<object> Post(BatchUpdateClockingStatusAndBatchAddClockingHistoriesReq req)
        {
            var returnObj = await _mediator.Send(new BatchUpdateClockingCommand(req.Clockings, req.ClockingHistory, req.LeaveOfAbsence));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(returnObj);
        }

        public async Task<object> Post(AutomatedTimeKeepingReq req)
        {
            var returnObj = await _mediator.Send(new UpdateWhenUseAutomatedTimeKeepingCommand(req.EmployeeId, req.TimeKeepingDate));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(returnObj);
        }

        public async Task<object> Post(AutomatedMultipleTimeKeepingReq req)
        {
            var returnObj = await _mediator.Send(new BatchUpdateWhenUseAutomatedTimeKeepingCommand(req.AutomatedTimekeepingDtos));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(returnObj);
        }

        #endregion

        #region PUT methods
        public async Task<object> Put(UpdateClockingReq req)
        {
            var returnObj = await _mediator.Send(new UpdateClockingCommand(req.Clocking, req.ReplacementEmployeeId, req.ClockingHistory, req.LeaveOfAbsence));

            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(returnObj);
        }

        public async Task<object> Put(UpdateClockingShiftAndDateTimeReq req)
        {
            var returnObj = await _mediator.Send(new UpdateClockingMultipleBranchShiftAndDateTimeCommand(req.Id, req.ShiftTargetId, req.BranchId, req.WorkingDay));
            if (Errors.Any())
            {
                return BadRequest(Message.error_whenUpdateData, Errors);
            }

            return Ok(returnObj);
        }
        #endregion
    }
}