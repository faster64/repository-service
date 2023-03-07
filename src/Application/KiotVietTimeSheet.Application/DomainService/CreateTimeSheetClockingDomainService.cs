using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.Results;
using KiotVietTimeSheet.Application.Configuration;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Application.DomainService.Enums;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.TimeSheetValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Utilities;

namespace KiotVietTimeSheet.Application.DomainService
{
    public class CreateTimeSheetClockingDomainService : ICreateTimeSheetClockingDomainService
    {
        #region Properties
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly ITimeSheetWriteOnlyRepository _timeSheetWriteOnlyRepository;
        private readonly IGenerateClockingsDomainService _generateClockingsDomainService;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IApplicationConfiguration _applicationConfiguration;
        #endregion

        #region Constructor
        public CreateTimeSheetClockingDomainService(
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            ITimeSheetWriteOnlyRepository timeSheetWriteOnlyRepository,
            IGenerateClockingsDomainService generateClockingsDomainService,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IMapper mapper,
            IApplicationConfiguration applicationConfiguration)
        {
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _timeSheetWriteOnlyRepository = timeSheetWriteOnlyRepository;
            _generateClockingsDomainService = generateClockingsDomainService;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _mapper = mapper;
            _applicationConfiguration = applicationConfiguration;
        }
        #endregion

        #region Public methods
        public async Task<TimeSheetDomainServiceDto> CreateAsync(TimeSheetDto timeSheetDto)
        {
            var generatedData = await GenerateTimeSheetAndClocking(timeSheetDto);
            if (!generatedData.IsValid)
            {
                return new TimeSheetDomainServiceDto
                {
                    TimeSheets = null,
                    IsValid = false,
                    ValidationErrors = generatedData.ValidationErrors
                };
            }
            var validateResult = await (new CreateTimeSheetValidator(generatedData.ClockingsOverlapTime, generatedData.TimeSheetClockings, _applicationConfiguration.TimeSheetValidate))
                .ValidateAsync(generatedData.TimeSheets);

            return ValidateAndCreate(generatedData, validateResult);
        }

        public async Task<TimeSheetDomainServiceDto> CreateWhenReplaceEmployeeAsync(TimeSheetDto timeSheetDto)
        {
            var generatedData = await GenerateTimeSheetAndClocking(timeSheetDto);
            if (!generatedData.IsValid)
            {
                return new TimeSheetDomainServiceDto
                {
                    TimeSheets = null,
                    IsValid = false,
                    ValidationErrors = generatedData.ValidationErrors
                };
            }
            var validateResult = await (new CreateTimeSheetClockingWhenReplaceEmployeeValidator(_employeeReadOnlyRepository, _shiftReadOnlyRepository, generatedData.ClockingsOverlapTime, generatedData.TimeSheetClockings))
                .ValidateAsync(generatedData.TimeSheets);

            return ValidateAndCreate(generatedData, validateResult);
        }
        #endregion

        #region Private methods
        private async Task<GenerateClockingsDomainServiceDto> GenerateTimeSheetAndClocking(TimeSheetDto timeSheetDto)
        {
            var timeSheetShifts = new List<TimeSheetShift>();
            foreach (var timeSheetShift in timeSheetDto.TimeSheetShifts)
            {
                timeSheetShifts.Add(new TimeSheetShift(timeSheetShift.TimeSheetId, timeSheetShift.ShiftIds,
                    timeSheetShift.RepeatDaysOfWeek));

            }

            var autoGenerateClockingStatus = (byte) Globals.GetAutoGenerateClockingStatus(timeSheetDto.IsRepeat ?? false, timeSheetDto.HasEndDate);
            var timeSheet = new TimeSheet(
                timeSheetDto.EmployeeId,
                timeSheetDto.StartDate,
                timeSheetDto.IsRepeat,
                timeSheetDto.RepeatType,
                timeSheetDto.RepeatEachDay,
                timeSheetDto.EndDate,
                timeSheetDto.BranchId,
                timeSheetDto.SaveOnDaysOffOfBranch,
                timeSheetDto.SaveOnHoliday,
                timeSheetShifts,
                timeSheetDto.Note,
                autoGenerateClockingStatus
            );

            var generateClockingForTimeSheetsDto = new GenerateClockingForTimeSheetsDto()
            {
                TimeSheets = new List<TimeSheet> { timeSheet },
                GenerateByType = GenerateClockingByType.TimeSheet
            };

            return await _generateClockingsDomainService.GenerateClockingForTimeSheets(generateClockingForTimeSheetsDto);
        }

        private TimeSheetDomainServiceDto ValidateAndCreate(GenerateClockingsDomainServiceDto data, ValidationResult validateResult)
        {
            TimeSheetDto timeSheetMapping = null;
            if (validateResult.IsValid)
            {
                var timeSheet = AddToEFContext(data.TimeSheets, data.TimeSheetClockings);
                timeSheetMapping = _mapper.Map<TimeSheetDto>(timeSheet);
                timeSheetMapping.Clockings = data.TimeSheetClockings;
                if (data.ClockingsOverlapTime.Any())
                {
                    timeSheetMapping.ClockingsOverlapTime = data.ClockingsOverlapTime;
                }
            }

            var result = new TimeSheetDomainServiceDto
            {
                TimeSheets = new List<TimeSheetDto> { timeSheetMapping },
                IsValid = validateResult.IsValid,
                ValidationErrors = validateResult.Errors.Select(e => e.ErrorMessage).ToList()
            };

            return result;
        }

        private TimeSheet AddToEFContext(List<TimeSheet> timeSheets, List<Clocking> timeSheetClockings)
        {
            List<Clocking> clockings = new List<Clocking>();
            // Chỉ thêm mới lịch làm việc khi có chi tiết làm việc
            var timeSheet = timeSheets.FirstOrDefault(ts =>
                timeSheetClockings != null &&
                timeSheetClockings.Any(x => x.TimeSheetId == (ts.Id > 0 ? ts.Id : ts.TemporaryId)));
            if (timeSheet != null)
            {
                _timeSheetWriteOnlyRepository.Add(
                    timeSheet
                );

                var clockingsNeedAdd = timeSheetClockings.Where(c => c.Id <= 0).ToList();
                clockingsNeedAdd.ForEach(clocking => { clocking.UpdateClockingTimeSheetId(timeSheet.Id); });
                clockings.AddRange(clockingsNeedAdd);
                _clockingWriteOnlyRepository.BatchAdd(clockings);
            }

            return timeSheet;
        }
        #endregion



    }
}
