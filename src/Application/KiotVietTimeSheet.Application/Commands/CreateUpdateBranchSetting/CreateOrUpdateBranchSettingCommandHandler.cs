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
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Validations;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CreateUpdateBranchSetting
{
    public class CreateOrUpdateBranchSettingCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateOrUpdateBranchSettingCommand, BranchSettingDto>
    {
        private readonly IBranchSettingWriteOnlyRepository _branchSettingWriteOnlyRepository;
        private readonly IGetTimeSheetByBranchWorkingDaysDomainService _getTimeSheetByBranchWorkingDaysDomainService;
        private readonly IBatchUpdateTimeSheetClockingsDomainService _batchUpdateTimeSheetClockingsDomainService;
        private readonly IMapper _mapper;

        public CreateOrUpdateBranchSettingCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IBranchSettingWriteOnlyRepository branchSettingWriteOnlyRepository,
            IGetTimeSheetByBranchWorkingDaysDomainService getTimeSheetByBranchWorkingDaysDomainService,
            IBatchUpdateTimeSheetClockingsDomainService batchUpdateTimeSheetClockingsDomainService
        ) : base(eventDispatcher)
        {
            _branchSettingWriteOnlyRepository = branchSettingWriteOnlyRepository;
            _getTimeSheetByBranchWorkingDaysDomainService = getTimeSheetByBranchWorkingDaysDomainService;
            _batchUpdateTimeSheetClockingsDomainService = batchUpdateTimeSheetClockingsDomainService;
            _mapper = mapper;
        }

        public async Task<BranchSettingDto> Handle(CreateOrUpdateBranchSettingCommand request,
            CancellationToken cancellationToken)
        {
            var branchSettingDto = request.BranchSettingDto;
            var isAddMore = request.IsAddMore;
            var isRemove = request.IsRemove;
            var applyFrom = request.ApplyFrom;
            var branchSetting =
                await _branchSettingWriteOnlyRepository.FindBySpecificationAsync(
                    new FindBranchSettingByBranchIdSpec(branchSettingDto.BranchId));
            var isAddNewBranchSetting = false;
            if (branchSetting == null)
            {
                isAddNewBranchSetting = true;
                branchSetting = new BranchSetting(branchSettingDto.BranchId, BranchSetting.DefaultWorkingDays.ToList());
                _branchSettingWriteOnlyRepository.Add(branchSetting);
            }

            if (isAddMore || isRemove)
            {
                applyFrom = applyFrom.Date;
                var workingDaysChanged = GetWorkingDaysChanged(branchSettingDto, branchSetting, isAddMore, isRemove);

                var getTimeSheetByBranchWorkingDaysDto = new GetTimeSheetByBranchWorkingDaysDto()
                {
                    ApplyFrom = applyFrom,
                    BranchId = branchSetting.BranchId,
                    WorkingDays = workingDaysChanged
                };
                var timeSheets =
                    _getTimeSheetByBranchWorkingDaysDomainService.GetTimeSheetByBranchWorkingDay(
                        getTimeSheetByBranchWorkingDaysDto);

                if (timeSheets != null && timeSheets.Any())
                {
                    var generateClockingForTimeSheetsDto = new GenerateClockingForTimeSheetsDto()
                    {
                        TimeSheets = timeSheets,
                        IsAddClockings = isAddMore,
                        IsRemoveClockings = isRemove,
                        ApplyTimes = new List<DateRangeDto>()
                        {
                            new DateRangeDto()
                            {
                                From = applyFrom
                            }
                        },
                        WorkingDays = branchSettingDto.WorkingDays,
                        WorkingDaysChanged = workingDaysChanged,
                        GenerateByType = GenerateClockingByType.BranchWorkingDay
                    };
                    var result =
                        await _batchUpdateTimeSheetClockingsDomainService
                            .BatchUpdateTimeSheetClockingWhenUpdateDaysOffAsync(generateClockingForTimeSheetsDto);
                    if (!result.IsValid)
                    {
                        NotifyValidationErrors(typeof(TimeSheet), result.ValidationErrors);
                        return _mapper.Map<BranchSettingDto>(branchSetting);
                    }
                }
            }

            branchSetting.Update(branchSettingDto.WorkingDays);

            if (!isAddNewBranchSetting)
            {
                _branchSettingWriteOnlyRepository.Update(branchSetting);
            }

            var validateResult = await (new CreateOrUpdateBranchSettingValidator()).ValidateAsync(branchSetting, cancellationToken);

            if (!validateResult.IsValid)
            {
                NotifyValidationErrors(typeof(BranchSetting),
                    validateResult.Errors.Select(e => e.ErrorMessage).ToList());
                return _mapper.Map<BranchSettingDto>(branchSetting);
            }

            await _branchSettingWriteOnlyRepository.UnitOfWork.CommitAsync();

            return _mapper.Map<BranchSettingDto>(branchSetting);
        }

        #region Private method
        private List<byte> GetWorkingDaysChanged(BranchSettingDto branchSettingDto, BranchSetting branchSetting, bool isAddMore, bool isRemove)
        {
            var workingDaysChanged = new List<byte>();
            var dayNeedAdd = branchSettingDto.WorkingDays.Except(branchSetting.WorkingDaysInArray).ToList();
            var dayNeedRemove = branchSetting.WorkingDaysInArray.Except(branchSettingDto.WorkingDays).ToList();
            if (isAddMore)
            {
                workingDaysChanged.AddRange(dayNeedAdd);
            }

            if (isRemove)
            {
                workingDaysChanged.AddRange(dayNeedRemove);
            }

            return workingDaysChanged;
        }

        #endregion
    }
}
