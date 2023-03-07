using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Configuration;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Application.DomainService.Enums;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.TimeSheetValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.Utilities;
using MediatR;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Commands.BatchAddTimeSheetWhenCreateMultipleTimeSheet
{
    public class BatchAddTimeSheetWhenCreateMultipleTimeSheetCommandHandler : BaseCommandHandler,
        IRequestHandler<BatchAddTimeSheetWhenCreateMultipleTimeSheetCommand, List<TimeSheetDto>>
    {
        private readonly IGenerateClockingsDomainService _generateClockingsDomainService;
        private readonly ITimeSheetWriteOnlyRepository _timeSheetWriteOnlyRepository;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly ITimeSheetShiftWriteOnlyRepository _timeSheetShiftWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IMapper _mapper;
        private readonly IApplicationConfiguration _applicationConfiguration;

        public BatchAddTimeSheetWhenCreateMultipleTimeSheetCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IGenerateClockingsDomainService generateClockingsDomainService,
            ITimeSheetWriteOnlyRepository timeSheetWriteOnlyRepository,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            ITimeSheetShiftWriteOnlyRepository timeSheetShiftWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IMapper mapper,
            IApplicationConfiguration applicationConfiguration)
            : base(eventDispatcher)
        {
            _generateClockingsDomainService = generateClockingsDomainService;
            _timeSheetWriteOnlyRepository = timeSheetWriteOnlyRepository;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _timeSheetShiftWriteOnlyRepository = timeSheetShiftWriteOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _mapper = mapper;
            _applicationConfiguration = applicationConfiguration;
        }

        public async Task<List<TimeSheetDto>> Handle(BatchAddTimeSheetWhenCreateMultipleTimeSheetCommand request, CancellationToken cancellationToken)
        {
            var timeSheetDto = request.TimeSheet;
            var listEmployeeIds = request.EmployeeIds;
            var listTimeSheetShifts = new List<TimeSheetShift>();
            foreach (var timeSheetShift in timeSheetDto.TimeSheetShifts)
            {
                listTimeSheetShifts.Add(new TimeSheetShift(timeSheetShift.TimeSheetId, timeSheetShift.ShiftIds, timeSheetShift.RepeatDaysOfWeek));

            }
            var autoGenerateClockingStatus = (byte) Globals.GetAutoGenerateClockingStatus(request.TimeSheet.IsRepeat ?? false, request.TimeSheet.HasEndDate);
            var listTimeSheets = new List<TimeSheet>();
            var firstTimeSheet = new TimeSheet(
                listEmployeeIds != null && listEmployeeIds.Any() ? listEmployeeIds.FirstOrDefault() : timeSheetDto.EmployeeId,
                timeSheetDto.StartDate,
                timeSheetDto.IsRepeat,
                timeSheetDto.RepeatType,
                timeSheetDto.RepeatEachDay,
                timeSheetDto.EndDate,
                timeSheetDto.BranchId,
                timeSheetDto.SaveOnDaysOffOfBranch,
                timeSheetDto.SaveOnHoliday,
                listTimeSheetShifts,
                timeSheetDto.Note,
                autoGenerateClockingStatus
            );
            listTimeSheets.Add(firstTimeSheet);

            if (listEmployeeIds != null && listEmployeeIds.Any())
            {
                foreach (var employeeIdItem in listEmployeeIds.Where(e => e != firstTimeSheet.EmployeeId).Distinct())
                {
                    var newListTimeSheetShifts = new List<TimeSheetShift>();
                    foreach (var timeSheetShift in timeSheetDto.TimeSheetShifts)
                    {
                        newListTimeSheetShifts.Add(new TimeSheetShift(timeSheetShift.TimeSheetId, timeSheetShift.ShiftIds, timeSheetShift.RepeatDaysOfWeek));

                    }
                    var newTimeSheet = firstTimeSheet.CloneTimeSheet(firstTimeSheet);
                    newTimeSheet.TimeSheetShifts = newListTimeSheetShifts;
                    newTimeSheet.SetEmployeeId(employeeIdItem);
                    listTimeSheets.Add(newTimeSheet);
                }
            }

            var generateClockingForTimeSheetsDtoItem = new GenerateClockingForTimeSheetsDto()
            {
                TimeSheets = listTimeSheets,
                GenerateByType = GenerateClockingByType.TimeSheet
            };
            var generatedDataItem = await _generateClockingsDomainService.GenerateClockingForTimeSheets(generateClockingForTimeSheetsDtoItem);

            // Validate TimeSheet
            if (!generatedDataItem.IsValid)
            {
                NotifyValidationErrors(typeof(TimeSheet), generatedDataItem.ValidationErrors);
                return null;
            }

            var validator = await (new BatchAddUpdateTimeSheetsValidator(generatedDataItem.ClockingsOverlapTime, generatedDataItem.TimeSheetClockings, _applicationConfiguration.TimeSheetValidate)).ValidateAsync(generatedDataItem.TimeSheets);
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(TimeSheet), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }
            // End

            await BatchAddAsync(timeSheetDto, generatedDataItem.TimeSheets, generatedDataItem.TimeSheetClockings, request.IsAuto);
            await _timeSheetWriteOnlyRepository.UnitOfWork.CommitAsync();

            var result = _mapper.Map<List<TimeSheetDto>>(generatedDataItem.TimeSheets);
            foreach (var timeSheet in result)
            {
                timeSheet.Clockings = generatedDataItem.TimeSheetClockings.Where(x => x.TimeSheetId == timeSheet.Id).ToList();
                if (generatedDataItem.ClockingsOverlapTime.Any())
                {
                    timeSheet.ClockingsOverlapTime = generatedDataItem.ClockingsOverlapTime
                        .Where(x => x.EmployeeId == timeSheet.EmployeeId).ToList();
                }
            }
            return result;
        }

        private async Task<List<TimeSheet>> BatchAddAsync(TimeSheetDto timeSheetDto, List<TimeSheet> listTimeSheets, List<Clocking> timeSheetClockings, bool isAuto)
        {
            var clockings = new List<Clocking>();
            var timeSheetShift = new List<TimeSheetShift>();
            // Chỉ thêm mới lịch làm việc khi có chi tiết làm việc
            var timeSheetsValid = listTimeSheets.Where(ts =>
                timeSheetClockings != null &&
                timeSheetClockings.Any(x => x.TimeSheetId == (ts.Id > 0 ? ts.Id : ts.TemporaryId))).ToList();

            _timeSheetWriteOnlyRepository.BatchAdd(
                timeSheetsValid
            );

            foreach (var timeSheetItem in timeSheetsValid)
            {
                var clockingsNeedAdd = timeSheetClockings.Where(c =>
                    c.Id <= 0 && c.TimeSheetId == timeSheetItem.TemporaryId).ToList();
                clockingsNeedAdd.ForEach(clocking => { clocking.UpdateClockingTimeSheetId(timeSheetItem.Id); });
                clockings.AddRange(clockingsNeedAdd);

                timeSheetItem.TimeSheetShifts.ForEach(t => { t.UpdateTimeSheetId(timeSheetItem.Id); });
                timeSheetShift.AddRange(timeSheetItem.TimeSheetShifts);
            }

            _clockingWriteOnlyRepository.BatchAdd(clockings);
            _timeSheetShiftWriteOnlyRepository.BatchAdd(timeSheetShift);

            timeSheetDto.TimeSheetShifts = timeSheetShift.ConvertTo<List<TimeSheetShiftDto>>();
            await _timeSheetIntegrationEventService.AddEventAsync(new CreateMultipleClockingIntegrationEvent(timeSheetDto, clockings, !isAuto));

            return timeSheetsValid;
        }

    }
}
