using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Configuration;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.TimeSheetValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.EventBus;
using KiotVietTimeSheet.Utilities;

namespace KiotVietTimeSheet.Application.DomainService
{
    public class BatchUpdateTimeSheetClockingsDomainService : IBatchUpdateTimeSheetClockingsDomainService
    {
        #region PROPERTIES
        private readonly IGenerateClockingsDomainService _generateClockingsDomainService;
        private readonly ITimeSheetWriteOnlyRepository _timeSheetWriteOnlyRepository;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly ITimeSheetShiftWriteOnlyRepository _timeSheetShiftWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly IMapper _mapper;
        #endregion

        #region CONSTRUCTORS

        public BatchUpdateTimeSheetClockingsDomainService(
            IGenerateClockingsDomainService generateClockingsDomainService,
            ITimeSheetWriteOnlyRepository timeSheetWriteOnlyRepository,
            ITimeSheetShiftWriteOnlyRepository timeSheetShiftWriteOnlyRepository,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            IMapper mapper,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IApplicationConfiguration applicationConfiguration)
        {
            _generateClockingsDomainService = generateClockingsDomainService;
            _timeSheetWriteOnlyRepository = timeSheetWriteOnlyRepository;
            _timeSheetShiftWriteOnlyRepository = timeSheetShiftWriteOnlyRepository;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _mapper = mapper;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _applicationConfiguration = applicationConfiguration;
        }

        #endregion

        #region METHODS

        public async Task<TimeSheetDomainServiceDto> BatchUpdateTimeSheetClockingWhenUpdateDaysOffAsync(GenerateClockingForTimeSheetsDto generateClockingForTimeSheetsDto)
        {
            var result = new TimeSheetDomainServiceDto
            {
                TimeSheets = null,
                IsValid = false,
                ValidationErrors = new[] { Message.timeSheet_notExist }
            };

            if (generateClockingForTimeSheetsDto.TimeSheets == null || !generateClockingForTimeSheetsDto.TimeSheets.Any() || generateClockingForTimeSheetsDto.GenerateByType == null)
            {
                return result;
            }
            var generatedData = await _generateClockingsDomainService.GenerateClockingForTimeSheets(generateClockingForTimeSheetsDto);
            result.TimeSheets = _mapper.Map<List<TimeSheetDto>>(generatedData.TimeSheets);

            // Validate TimeSheet
            if (!generatedData.IsValid)
            {
                result.ValidationErrors = generatedData.ValidationErrors;
            }
            else
            {
                var validatorResult = await (new BatchUpdateTimeSheetsWhenUpdateDaysOffValidator(_applicationConfiguration.TimeSheetValidate)).ValidateAsync(generatedData.TimeSheets);
                if (validatorResult.IsValid)
                {
                    await BatchUpdateTimeSheetAndClockingWhenChangeDayOffAsync(generatedData.TimeSheets, generatedData.TimeSheetClockings, generatedData.ClockingNeedUpdateIds);
                }

                result.IsValid = validatorResult.IsValid;
                result.ValidationErrors = validatorResult.Errors.Select(e => e.ErrorMessage).ToList();
            }
            // End

            return result;
        }

        public async Task BatchUpdateTimeSheetAndClocking(List<TimeSheet> timeSheets, List<Clocking> timeSheetClockings,
            List<long> clockingUpdateIds, (TimeSheetDto, TimeSheet, bool) values)
        {
            var clockingsNeedAdd = new List<Clocking>();
            var clockingsNeedUpdate = new List<Clocking>();
            var timeSheetShiftAdd = new List<TimeSheetShift>();
            var timeSheetIds = timeSheets.Select(x => x.Id).ToList();
            var timeSheetShiftDelete = await
                _timeSheetShiftWriteOnlyRepository.GetBySpecificationAsync(new FindTimeSheetShiftByTimeSheetIdsSpec(timeSheetIds));
            foreach (var timeSheet in timeSheets)
            {
                timeSheet.TimeSheetShifts.ForEach(t =>
                {
                    t.Id = 0;
                    t.UpdateTimeSheetId(timeSheet.Id);
                });
                timeSheetShiftAdd.AddRange(timeSheet.TimeSheetShifts);

                var newLsClocking = timeSheetClockings.Where(c =>
                    c.Id <= 0 && c.TimeSheetId == (timeSheet.Id > 0 ? timeSheet.Id : timeSheet.TemporaryId)).ToList();
                newLsClocking.ForEach(clocking => { clocking.UpdateClockingTimeSheetId(timeSheet.Id); });
                clockingsNeedAdd.AddRange(newLsClocking);

            }
            clockingsNeedUpdate.AddRange(timeSheetClockings.Where(c => clockingUpdateIds.Contains(c.Id)).ToList());

            var detachedLsTimeSheet = _timeSheetWriteOnlyRepository.BatchDetachByClone(timeSheets).ToList();
            _timeSheetWriteOnlyRepository.BatchUpdate(detachedLsTimeSheet);
            _clockingWriteOnlyRepository.BatchAdd(clockingsNeedAdd);
            _clockingWriteOnlyRepository.BatchUpdate(clockingsNeedUpdate);

            _timeSheetShiftWriteOnlyRepository.BatchDelete(timeSheetShiftDelete);
            _timeSheetShiftWriteOnlyRepository.BatchAdd(timeSheetShiftAdd);

            var (timeSheetDto, timeSheetOld, forAllClockings) = values;
            var listIntegrationEvents = new List<IntegrationEvent>();
            if (clockingsNeedAdd.Any())
            {
                listIntegrationEvents.Add(new CreateMultipleClockingIntegrationEvent(timeSheetDto, timeSheetOld, clockingsNeedAdd, forAllClockings, true));
            }
            if (clockingsNeedUpdate.Any())
            {
                listIntegrationEvents.Add(new RejectMultipleClockingIntegrationEvent(timeSheetDto, timeSheetOld, clockingsNeedUpdate, true));
            }
            listIntegrationEvents.AddRange(GetEventLogIfHaveChangeInfo(timeSheetDto, timeSheetOld, clockingsNeedAdd.Concat(clockingsNeedUpdate).ToList(), forAllClockings));
            if (listIntegrationEvents.Any())
            {
                await _timeSheetIntegrationEventService.AddMultiEventAsync(listIntegrationEvents);
            }
        }
        public async Task BatchUpdateTimeSheetAndClockingWhenChangeDayOffAsync(List<TimeSheet> timeSheets, List<Clocking> timeSheetClockings,
           List<long> clockingUpdateIds)
        {
            var clockingsNeedAdd = new List<Clocking>();
            var clockingsNeedUpdate = new List<Clocking>();


            foreach (var timeSheet in timeSheets)
            {
                var newTimeSheetShift = timeSheet.TimeSheetShifts.Where(c => c.Id <= 0).ToList();
                newTimeSheetShift.ForEach(t => { t.UpdateTimeSheetId(timeSheet.Id); });

                var newClockings = timeSheetClockings.Where(c =>
                    c.Id <= 0 && c.TimeSheetId == (timeSheet.Id > 0 ? timeSheet.Id : timeSheet.TemporaryId)).ToList();
                newClockings.ForEach(clocking => { clocking.UpdateClockingTimeSheetId(timeSheet.Id); });
                clockingsNeedAdd.AddRange(newClockings);

            }
            clockingsNeedUpdate.AddRange(timeSheetClockings.Where(c => clockingUpdateIds.Contains(c.Id)).ToList());

            var detachedTimeSheets = _timeSheetWriteOnlyRepository.BatchDetachByClone(timeSheets).ToList();
            _timeSheetWriteOnlyRepository.BatchUpdate(detachedTimeSheets);
            _clockingWriteOnlyRepository.BatchAdd(clockingsNeedAdd);
            _clockingWriteOnlyRepository.BatchUpdate(clockingsNeedUpdate);

            var listIntegrationEvents = new List<IntegrationEvent>();
            if (clockingsNeedAdd.Any())
            {
                listIntegrationEvents.Add(new CreateMultipleClockingIntegrationEvent(clockingsNeedAdd));
            }
            if (clockingsNeedUpdate.Any())
            {
                // Khởi tạo event mới để lấy trạng thái clocking trước khi hủy để ghi vào audit log
                var clockingsNeedUpdateIntegrationEvent = clockingsNeedUpdate.Select(x => x.CreateCopy()).ToList();
                foreach (var clocking in clockingsNeedUpdateIntegrationEvent)
                {
                    clocking.UpdateClockingStatus((byte)ClockingStatuses.Created);
                }
                listIntegrationEvents.Add(new RejectMultipleClockingIntegrationEvent(clockingsNeedUpdateIntegrationEvent));
            }

            if (listIntegrationEvents.Any())
            {
                await _timeSheetIntegrationEventService.AddMultiEventAsync(listIntegrationEvents);
            }

        }
        #endregion

        #region PRIVATE

        private IEnumerable<IntegrationEvent> GetEventLogIfHaveChangeInfo(TimeSheetDto timeSheetDto, TimeSheet timeSheetOld, List<Clocking> clockings, bool forAllClockings)
        {
            var listIntegrationEvents = new List<IntegrationEvent>();
            if (!clockings.Any() && timeSheetDto != null && timeSheetOld != null && timeSheetDto.HasEndDate != (timeSheetOld.AutoGenerateClockingStatus != (byte)AutoGenerateClockingStatus.Auto))
            {
                listIntegrationEvents.Add(new ChangedClockingIntegrationEvent(timeSheetDto, timeSheetOld, forAllClockings, true));
            }
            if (clockings.Any() && timeSheetDto != null && timeSheetOld != null && timeSheetOld.AutoGenerateClockingStatus == (byte)AutoGenerateClockingStatus.Auto)
            {
                _timeSheetIntegrationEventService.PublishEventWithContext(new CreateMultipleClockingIntegrationEvent(timeSheetDto, timeSheetOld, clockings, forAllClockings, fromAuto: true));
            }
            return listIntegrationEvents;
        }

        #endregion
    }
}
