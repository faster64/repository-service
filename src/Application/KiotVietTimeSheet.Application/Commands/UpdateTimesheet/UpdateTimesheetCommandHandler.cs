using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Configuration;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Application.DomainService.Enums;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.TimeSheetValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Utilities;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UpdateTimesheet
{
    public class UpdateTimesheetCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateTimesheetCommand, TimeSheetDto>
    {
        private readonly IMapper _mapper;
        private readonly ITimeSheetWriteOnlyRepository _timeSheetWriteOnlyRepository;
        private readonly IGenerateClockingsDomainService _generateClockingsDomainService;
        private readonly IBatchUpdateTimeSheetClockingsDomainService _batchUpdateTimeSheetClockingsDomainService;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IApplicationConfiguration _applicationConfiguration;

        public UpdateTimesheetCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            ITimeSheetWriteOnlyRepository timeSheetWriteOnlyRepository,
            IGenerateClockingsDomainService generateClockingsDomainService,
            IBatchUpdateTimeSheetClockingsDomainService batchUpdateTimeSheetClockingsDomainService,
            IApplicationConfiguration applicationConfiguration)
            : base(eventDispatcher)
        {
            _mapper = mapper;
            _eventDispatcher = eventDispatcher;
            _timeSheetWriteOnlyRepository = timeSheetWriteOnlyRepository;
            _generateClockingsDomainService = generateClockingsDomainService;
            _batchUpdateTimeSheetClockingsDomainService = batchUpdateTimeSheetClockingsDomainService;
            _applicationConfiguration = applicationConfiguration;
        }

        public async Task<TimeSheetDto> Handle(UpdateTimesheetCommand request, CancellationToken cancellationToken)
        {
            var timeSheetDto = request.TimeSheet;
            var forAllClockings = request.ForAllClockings;
            var timeSheetSpecification = await _timeSheetWriteOnlyRepository.FindBySpecificationAsync(new FindTimeSheetByIdSpec(timeSheetDto.Id).Not(new FindTimeSheetByStatusSpec((byte)TimeSheetStatuses.Void)), "TimeSheetShifts");
            var timeSheetOld = new TimeSheet(timeSheetSpecification);

            if (timeSheetSpecification == null)
            {
                NotifyTimeSheetInDbIsNotExists();
                return null;
            }
            var timeSheetShifts = new List<TimeSheetShift>();
            foreach (var timeSheetShift in timeSheetDto.TimeSheetShifts)
            {
                timeSheetShifts.Add(new TimeSheetShift(timeSheetShift.TimeSheetId, timeSheetShift.ShiftIds, timeSheetShift.RepeatDaysOfWeek));
            }

            var autoGenerateClockingStatus = (byte) Globals.GetAutoGenerateClockingStatus(request.TimeSheet.IsRepeat ?? false, request.TimeSheet.HasEndDate);
            timeSheetSpecification.Update(
                timeSheetDto.StartDate,
                timeSheetDto.IsRepeat,
                timeSheetDto.RepeatType,
                timeSheetDto.RepeatEachDay,
                timeSheetDto.EndDate,
                timeSheetDto.SaveOnDaysOffOfBranch,
                timeSheetDto.SaveOnHoliday,
                timeSheetDto.Note,
                autoGenerateClockingStatus,
                timeSheetShifts
            );

            var generateClockingForTimeSheetsDto = new GenerateClockingForTimeSheetsDto()
            {
                TimeSheets = new List<TimeSheet>() { timeSheetSpecification },
                ApplyTimes = forAllClockings ? null : new List<DateRangeDto> { new DateRangeDto { From = DateTime.Now } },
                GenerateByType = GenerateClockingByType.TimeSheet
            };
            var generatedClockingData = await _generateClockingsDomainService.GenerateClockingForTimeSheets(generateClockingForTimeSheetsDto);

            // Validate TimeSheet
            if (!generatedClockingData.IsValid)
            {
                NotifyValidationErrors(typeof(TimeSheet), generatedClockingData.ValidationErrors);
                return null;
            }

            var validator = await (new UpdateTimeSheetValidator(generatedClockingData.ClockingsOverlapTime, generatedClockingData.TimeSheetClockings, _applicationConfiguration.TimeSheetValidate))
                .ValidateAsync(generatedClockingData.TimeSheets);
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(TimeSheet), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }
            // End
            var newClockingList = generatedClockingData.TimeSheetClockings.Where(x => x.Id == 0).ToList();
            await _batchUpdateTimeSheetClockingsDomainService.BatchUpdateTimeSheetAndClocking(
                generatedClockingData.TimeSheets, generatedClockingData.TimeSheetClockings,
                generatedClockingData.ClockingNeedUpdateIds, (timeSheetDto, timeSheetOld, request.ForAllClockings));
            await _timeSheetWriteOnlyRepository.UnitOfWork.CommitAsync();

            var result = _mapper.Map<TimeSheetDto>(generatedClockingData.TimeSheets.FirstOrDefault());
            result.Clockings = newClockingList;
            if (generatedClockingData.ClockingsOverlapTime.Any())
            {
                result.ClockingsOverlapTime = generatedClockingData.ClockingsOverlapTime;
            }
            return result;
        }

        private void NotifyTimeSheetInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(TimeSheet).Name, Message.timeSheet_notExist));
        }

    }
}
