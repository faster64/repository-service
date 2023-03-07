using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Application.DomainService.Enums;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.TimeSheetValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Commands.CopyListTimeSheet
{
    public class CopyListTimeSheetCommandHandler : BaseCommandHandler,
        IRequestHandler<CopyListTimeSheetCommand, List<TimeSheetDto>>
    {
        private readonly IMapper _mapper;
        private readonly ITimeSheetWriteOnlyRepository _timeSheetWriteOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly IGenerateClockingsDomainService _generateClockingsDomainService;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly ITimeSheetShiftWriteOnlyRepository _timeSheetShiftWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;

        public CopyListTimeSheetCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            ITimeSheetWriteOnlyRepository timeSheetWriteOnlyRepository,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            IGenerateClockingsDomainService generateClockingsDomainService,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            ITimeSheetShiftWriteOnlyRepository timeSheetShiftWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService
        )
            : base(eventDispatcher)
        {
            _mapper = mapper;
            _timeSheetWriteOnlyRepository = timeSheetWriteOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _generateClockingsDomainService = generateClockingsDomainService;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _timeSheetShiftWriteOnlyRepository = timeSheetShiftWriteOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task<List<TimeSheetDto>> Handle(CopyListTimeSheetCommand request, CancellationToken cancellationToken)
        {
            var listTimeSheets = await _timeSheetWriteOnlyRepository.GetBySpecificationAsync(
                new FindTimeSheetByBranchIdSpec(request.BranchId)
                    .And(new FindTimeSheetByStartDateGreaterOrEqualSpec(request.CopyFrom))
                    .And(new FindTimeSheetByEndDateLessThanOrEqualSpec(request.CopyTo))
                    .Not(new FindTimeSheetByStatusSpec((byte)TimeSheetStatuses.Void)), "TimeSheetShifts");

            var listErrors = new List<string>();
            if (!listTimeSheets.Any())
            {
                listErrors.Add(Message.timeSheet_noDataInRangeTimeSelectForCopy);
                NotifyValidationErrors(typeof(Clocking), listErrors);
                return null;
            }
            
            var listShiftIds = listTimeSheets.SelectMany(t => t.TimeSheetShifts.SelectMany(tss => tss.ShiftIdsToList)).Distinct().ToList();
            var listShifts = await _shiftReadOnlyRepository.GetBySpecificationAsync(
                    new FindShiftActiveSpec().And(new FindShiftsByIds(listShiftIds)));
            var timeSheetsSuitableForCopy = new List<TimeSheet>();

            // Initialized new time sheet and generate clockings
            foreach (var timeSheet in listTimeSheets)
            {
                // Tính khoảng thời gian sao chép
                var startDurationDays = (timeSheet.StartDate.Date - request.CopyFrom.Date).TotalDays;
                var endDurationDays = (request.CopyTo.Date - timeSheet.EndDate).TotalDays;

                var listTimeSheetShifts = new List<TimeSheetShift>();
                foreach (var timeSheetShift in timeSheet.TimeSheetShifts)
                {
                    var validShiftIds = timeSheetShift.ShiftIdsToList.Where(id => listShifts.Any(s => s.Id == id)).ToList();
                    if (validShiftIds.Any())
                    {
                        listTimeSheetShifts.Add(new TimeSheetShift(0,
                            validShiftIds.Join(","),
                            timeSheetShift.RepeatDaysOfWeek));
                    }
                }

                if (!listTimeSheetShifts.Any()) continue;
                timeSheet.CopyTimeSheet(
                    0,
                    request.PasteFrom.AddDays(startDurationDays),
                    request.PasteTo.AddDays(-endDurationDays),
                    listTimeSheetShifts
                );
                timeSheetsSuitableForCopy.Add(timeSheet);
            }

            var generateClockingForTimeSheetsDtoItem = new GenerateClockingForTimeSheetsDto()
            {
                TimeSheets = timeSheetsSuitableForCopy,
                GenerateByType = GenerateClockingByType.TimeSheet
            };
            var timeSheetGeneratedData = await _generateClockingsDomainService.GenerateClockingForTimeSheets(generateClockingForTimeSheetsDtoItem);
            // End

            if (timeSheetGeneratedData.TimeSheets != null)
            {
                timeSheetGeneratedData.TimeSheets = timeSheetGeneratedData.TimeSheets.Where(x => x.TimeSheetStatus != (byte)TimeSheetStatuses.Void).ToList();
            }

            // Validate clocking
            if (!timeSheetGeneratedData.IsValid)
            {
                NotifyValidationErrors(typeof(TimeSheet), timeSheetGeneratedData.ValidationErrors);
                return null;
            }

            var validator = await (new CopyTimeSheetValidator(timeSheetGeneratedData.ClockingsOverlapTime, timeSheetGeneratedData.TimeSheets, timeSheetGeneratedData.TimeSheetClockings, request.CopyFrom, request.CopyTo)).ValidateAsync(timeSheetGeneratedData.TimeSheets);
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(TimeSheet), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }
            // End

            timeSheetGeneratedData.TimeSheets = await BatchAddAsync(timeSheetGeneratedData.TimeSheets, timeSheetGeneratedData.TimeSheetClockings);
            await _timeSheetWriteOnlyRepository.UnitOfWork.CommitAsync();
            return _mapper.Map<List<TimeSheetDto>>(timeSheetGeneratedData.TimeSheets);
        }

        private async Task<List<TimeSheet>> BatchAddAsync(List<TimeSheet> timeSheets, List<Clocking> timeSheetClockings)
        {
            var listClocking = new List<Clocking>();
            var timeSheetShiftList = new List<TimeSheetShift>();
            // Chỉ thêm mới lịch làm việc khi có chi tiết làm việc
            var listTimeSheetValid = timeSheets.Where(ts =>
                timeSheetClockings != null &&
                timeSheetClockings.Any(x => x.TimeSheetId == (ts.Id > 0 ? ts.Id : ts.TemporaryId))).ToList();

            _timeSheetWriteOnlyRepository.BatchAdd(
                listTimeSheetValid
            );

            foreach (var timeSheet in listTimeSheetValid)
            {
                var clockingListNeedAdd = timeSheetClockings.Where(c =>
                    c.Id <= 0 && c.TimeSheetId == timeSheet.TemporaryId).ToList();
                clockingListNeedAdd.ForEach(clocking => { clocking.UpdateClockingTimeSheetId(timeSheet.Id); });
                listClocking.AddRange(clockingListNeedAdd);

                timeSheet.TimeSheetShifts.ForEach(t => { t.UpdateTimeSheetId(timeSheet.Id); });
                timeSheetShiftList.AddRange(timeSheet.TimeSheetShifts);
            }

            _clockingWriteOnlyRepository.BatchAdd(listClocking);
            _timeSheetShiftWriteOnlyRepository.BatchAdd(timeSheetShiftList);

            await _timeSheetIntegrationEventService.AddEventAsync(new CreateMultipleClockingIntegrationEvent(listClocking));

            return listTimeSheetValid;
        }
    }
}
