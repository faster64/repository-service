using System;
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
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.HolidayValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Commands.UpdateHoliday
{
    public class UpdateHolidayCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateHolidayCommand, HolidayDto>
    {
        private readonly IMapper _mapper;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly ITimeSheetReadOnlyRepository _timeSheetReadOnlyRepository;
        private readonly IHolidayWriteOnlyRepository _holidayWriteOnlyRepository;
        private readonly IBatchUpdateTimeSheetClockingsDomainService _batchUpdateTimeSheetClockingsDomainService;
        private readonly CreateOrUpdateHolidayAsyncValidator _createOrUpdateHolidayAsyncValidator;

        public UpdateHolidayCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            ITimeSheetReadOnlyRepository timeSheetReadOnlyRepository,
            IHolidayWriteOnlyRepository holidayWriteOnlyRepository,
            IBatchUpdateTimeSheetClockingsDomainService batchUpdateTimeSheetClockingsDomainService,
            CreateOrUpdateHolidayAsyncValidator createOrUpdateHolidayAsyncValidator
            ) : base(eventDispatcher)
        {
            _mapper = mapper;
            _eventDispatcher = eventDispatcher;
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _timeSheetReadOnlyRepository = timeSheetReadOnlyRepository;
            _holidayWriteOnlyRepository = holidayWriteOnlyRepository;
            _batchUpdateTimeSheetClockingsDomainService = batchUpdateTimeSheetClockingsDomainService;
            _createOrUpdateHolidayAsyncValidator = createOrUpdateHolidayAsyncValidator;
        }

        public bool CheckDateApply(DateRangeDto dateApply)
        {
            if (dateApply.From == null || dateApply.To == null)
            {
                NotifyValidationErrors(typeof(Holiday), new[] { Message.generalErrorMsg });
                return false;
            }
            return true;
        }

        public bool IsShowPopupOverLapClocking(UpdateHolidayCommand request , bool isCondition)
        {
            if (!request.IsShowPopupOverLapClocking && isCondition)
            {
                return false;
            }

            return true;
        }
        public async Task<HolidayDto> Handle(UpdateHolidayCommand request, CancellationToken cancellationToken)
        {
            var holidayDto = request.Holiday;
            var oldHoliday = await _holidayWriteOnlyRepository.FindByIdAsync(holidayDto.Id);
            if (oldHoliday == null)
            {
                NotifyUpdateHolidayInDbIsNotExists();
                return null;
            }

            var holidayUpdate = oldHoliday.CreateCopy();

            holidayDto.From = holidayDto.From.Date;
            holidayDto.To = holidayDto.To.Date;
            holidayUpdate.Update(holidayDto.Name, holidayDto.From, holidayDto.To);

            var validateResult = await _createOrUpdateHolidayAsyncValidator.ValidateAsync(holidayUpdate, cancellationToken);

            if (!validateResult.IsValid)
            {
                NotifyValidationErrors(typeof(Holiday), validateResult.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }

            var listTimeSheetUpdate = new List<TimeSheet>();
            var applyTimesCancelClocking = GetApplyTimesCancelClockingWhenUpdateHoliday(request.IsCancelClocking, request.IsShowPopupOverLapClocking, holidayUpdate, oldHoliday);
            var applyTimesAddClocking = GetApplyTimesAddClocking(request.IsAddClocking, request.IsShowPopupOverLapClocking, holidayUpdate, oldHoliday);

            foreach (var dateApply in applyTimesCancelClocking)
            {
                if (!CheckDateApply(dateApply)) return null;

                // case timesheet qua ngày nghỉ nhưng lại không có clocking nào trên ngày nghỉ, sẽ không cần thông báo và hủy clocking
                var clockingsNeedCancel = await _clockingReadOnlyRepository.GetClockingOverlapHoliday((DateTime)dateApply.From,
                    (DateTime)dateApply.To);

                
                if (!IsShowPopupOverLapClocking(request, clockingsNeedCancel.Any())) return null;

                var timeSheetsNeedCancel =
                    await _timeSheetReadOnlyRepository.GetTimeSheetOverlayHoliday((DateTime)dateApply.From,
                        (DateTime)dateApply.To);
                listTimeSheetUpdate.AddRange(timeSheetsNeedCancel.Where(t => !listTimeSheetUpdate.Select(x => x.Id).Contains(t.Id)));
            }

            foreach (var dateApply in applyTimesAddClocking)
            {
                if (!CheckDateApply(dateApply)) return null;

                var timeSheetsNeedAdd =
                    await _timeSheetReadOnlyRepository.GetTimeSheetOverlayHoliday((DateTime)dateApply.From,
                        (DateTime)dateApply.To);

               if (!IsShowPopupOverLapClocking(request, timeSheetsNeedAdd.Any())) return null;

               listTimeSheetUpdate.AddRange(timeSheetsNeedAdd.Where(t => !listTimeSheetUpdate.Select(x => x.Id).Contains(t.Id)));
            }

            var resultUpdate = await CheckResultUpdateValid(listTimeSheetUpdate, request, applyTimesCancelClocking,
                applyTimesAddClocking, holidayUpdate);
            if ( !resultUpdate)
                return null;

            _holidayWriteOnlyRepository.Update(holidayUpdate);

            await _holidayWriteOnlyRepository.UnitOfWork.CommitAsync();

            var result = _mapper.Map<HolidayDto>(holidayUpdate);
            return result;
        }

        private async Task<bool> CheckResultUpdateValid(List<TimeSheet> listTimeSheetUpdate, UpdateHolidayCommand request, List<DateRangeDto> applyTimesCancelClocking,
            List<DateRangeDto> applyTimesAddClocking, Holiday holidayUpdate)
        {
            if (listTimeSheetUpdate.Any())
            {
                var generateClockingForTimeSheetsDto = new GenerateClockingForTimeSheetsDto()
                {
                    TimeSheets = listTimeSheetUpdate,
                    IsAddClockings = request.IsAddClocking,
                    IsRemoveClockings = request.IsCancelClocking,
                    ApplyTimes = applyTimesCancelClocking.Concat(applyTimesAddClocking).ToList(),
                    Holidays = new List<Holiday> { holidayUpdate },
                    GenerateByType = GenerateClockingByType.Holiday
                };
                var resultUpdate = await _batchUpdateTimeSheetClockingsDomainService.BatchUpdateTimeSheetClockingWhenUpdateDaysOffAsync(generateClockingForTimeSheetsDto);

                if (!resultUpdate.IsValid)
                {
                    NotifyValidationErrors(typeof(TimeSheet), resultUpdate.ValidationErrors);
                    return false;
                }
            }

            return true;
        }

        #region Private Methods

        private void NotifyUpdateHolidayInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(Holiday).Name, Message.holiday_notExist));
        }

        // Tăng thêm thời gian của nghỉ lễ sẽ phải hủy bớt các chi tiết làm việc nếu ng dùng chấp nhận hủy
        // Chỉ lấy ra các khoảng thời gian tăng thêm
        private List<DateRangeDto> GetApplyTimesCancelClockingWhenUpdateHoliday(bool isCancelClocking, bool isShowPopupOverLapClocking, Holiday holidayUpdate, Holiday oldHoliday)
        {
            var listRangeDateApply = new List<DateRangeDto>();
            if (holidayUpdate.From.Date == oldHoliday.From.Date && holidayUpdate.To.Date == oldHoliday.To.Date)
                return listRangeDateApply;
            if (!isCancelClocking && isShowPopupOverLapClocking)
                return listRangeDateApply;

            if (holidayUpdate.From.Date > oldHoliday.To.Date || holidayUpdate.To.Date < oldHoliday.From.Date)
            {
                listRangeDateApply.Add(new DateRangeDto()
                {
                    From = holidayUpdate.From,
                    To = holidayUpdate.To
                });
            }
            else
            {
                if (holidayUpdate.From < oldHoliday.From)
                {
                    listRangeDateApply.Add(new DateRangeDto()
                    {
                        From = holidayUpdate.From,
                        To = oldHoliday.From.AddDays(-1)
                    });
                }

                if (holidayUpdate.To > oldHoliday.To)
                {
                    listRangeDateApply.Add(new DateRangeDto()
                    {
                        From = oldHoliday.To.AddDays(1),
                        To = holidayUpdate.To
                    });
                }
            }

            return listRangeDateApply;
        }

        // Giảm bớt thời gian nghỉ lễ sẽ tạo mới thêm chi tiết làm việc nếu ng dùng chấp nhận tạo thêm
        // Chỉ lấy ra các khoảng thời gian giảm đi
        private List<DateRangeDto> GetApplyTimesAddClocking(bool isAddClocking, bool isShowPopupOverLapClocking, Holiday holidayUpdate, Holiday oldHoliday)
        {
            var listRangeDateApply = new List<DateRangeDto>();
            if (holidayUpdate.From.Date == oldHoliday.From.Date && holidayUpdate.To.Date == oldHoliday.To.Date)
                return listRangeDateApply;
            if (!isAddClocking && isShowPopupOverLapClocking)
                return listRangeDateApply;

            if (holidayUpdate.From.Date > oldHoliday.To.Date || holidayUpdate.To.Date < oldHoliday.From.Date)
            {
                listRangeDateApply.Add(new DateRangeDto()
                {
                    From = oldHoliday.From,
                    To = oldHoliday.To
                });
            }
            else
            {
                if (holidayUpdate.From > oldHoliday.From)
                {
                    listRangeDateApply.Add(new DateRangeDto()
                    {
                        From = oldHoliday.From,
                        To = holidayUpdate.From.AddDays(-1)
                    });
                }

                if (holidayUpdate.To < oldHoliday.To)
                {
                    listRangeDateApply.Add(new DateRangeDto()
                    {
                        From = holidayUpdate.To.AddDays(1),
                        To = oldHoliday.To
                    });
                }
            }

            return listRangeDateApply;
        }
        #endregion
    }
}
