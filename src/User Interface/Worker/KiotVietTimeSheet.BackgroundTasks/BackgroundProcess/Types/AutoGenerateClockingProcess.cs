using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.Commands.UpdateTimesheet;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Utilities;
using MediatR;
using Serilog;
using ServiceStack;

namespace KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types
{
    public class AutoGenerateClockingProcess : BaseBackgroundProcess
    {
        private readonly IMediator _mediator;
        private readonly ITimeSheetWriteOnlyRepository _timeSheetWriteOnlyRepository;
        private readonly ITimeSheetShiftWriteOnlyRepository _timeSheetShiftWriteOnlyRepository;
        private readonly IMapper _mapper;
        private readonly DomainNotificationHandler _notificationHandler;
        private readonly ICacheClient _cacheClient;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;

        public AutoGenerateClockingProcess(
            IMediator mediator,
            IKiotVietInternalService kiotVietInternalService,
            IAuthService authService,
            ITimeSheetWriteOnlyRepository timeSheetWriteOnlyRepository,
            IMapper mapper,
            ITimeSheetShiftWriteOnlyRepository timeSheetShiftWriteOnlyRepository,
            INotificationHandler<DomainNotification> notificationHandler,
            ICacheClient cacheClient,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository) : base(kiotVietInternalService,
            authService)
        {
            _timeSheetWriteOnlyRepository = timeSheetWriteOnlyRepository;
            _mapper = mapper;
            _timeSheetShiftWriteOnlyRepository = timeSheetShiftWriteOnlyRepository;
            _cacheClient = cacheClient;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _mediator = mediator;
            _notificationHandler = (DomainNotificationHandler)notificationHandler;
        }

        public async Task Create(CreateAutoGenerateClockingIntegrationEvent @event)
        {
            try
            {
                if (@event.HandleExpiredAt < DateTime.Now)
                {
                    Log.Information("Auto Generate Clocking ignore message by expire handle time");
                    return;
                }

                var expression = new FindTimeSheetAutoGenerateClockingSpec(@event.QuantityDayCondition);
                var timeSheets = await _timeSheetWriteOnlyRepository.GetBySpecificationAsync(expression);

                if (timeSheets?.Any() != true) return;

                var timeSheetIds = timeSheets.Select(x => x.Id).ToList();
                var timeSheetShifts = await _timeSheetShiftWriteOnlyRepository.GetBySpecificationAsync(new FindTimeSheetShiftByTimeSheetIdsSpec(timeSheetIds));

                var employeeIds = timeSheets.Select(x => x.EmployeeId).Distinct().ToList();
                var employeeIdWorkings = await _clockingWriteOnlyRepository.GetListEmployIdWorking(DateTime.Now.Date.AddDays(-@event.EmployeeWorkingInDay), DateTime.Now.Date, employeeIds);

                foreach (var timeSheet in timeSheets)
                {
                    if (employeeIdWorkings.All(employeeIdWorking => employeeIdWorking != timeSheet.EmployeeId))
                    {
                        Log.Information($"Auto Generate Clocking not valid by employee not working in {@event.EmployeeWorkingInDay} days [TimeSheet: {timeSheet.ToJson()}]");
                        continue;
                    }

                    var timeSheetDto = _mapper.Map<TimeSheetDto>(timeSheet);
                    var timeSheetShift = timeSheetShifts.Where(x => x.TimeSheetId == timeSheet.Id);

                    if (timeSheetShift?.Any() == true) timeSheetDto.TimeSheetShifts = _mapper.Map<List<TimeSheetShiftDto>>(timeSheetShift);

                    var totalDayAdd = (DateTime.Now.Date.AddDays(@event.QuantityDayAdd) - timeSheet.EndDate.Date).TotalDays;
                    if (totalDayAdd <= 0) continue;

                    timeSheetDto.EndDate = timeSheetDto.EndDate.AddDays(totalDayAdd);

                    await _mediator.Send(new UpdateTimesheetCommand(timeSheetDto, true));

                    var errors = _notificationHandler.Notifications.Select(n => n.ErrorResult).ToList();
                    if (errors?.Any() == true)
                    {
                        Log.Warning($"Auto Generate Clocking error by validate [TimeSheetId = {timeSheet.Id}]: {errors.ToJson()}");
                    }
                    else
                    {
                        Log.Information($"Auto Generate Clocking completed [TimeSheetId = {timeSheet.Id}]");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Auto Generate Clocking error");
                throw;
            }
            finally
            {
                var cacheKey = Globals.GetCacheAutoGenerateClockingInprogress(@event.Context.TenantId);
                _cacheClient.Remove(cacheKey);
            }
        }
    }
}
