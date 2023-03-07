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
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.DeleteHoliday
{
    public class DeleteHolidayCommandHandler : BaseCommandHandler,
        IRequestHandler<DeleteHolidayCommand, HolidayDto>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IMapper _mapper;
        private readonly ITimeSheetReadOnlyRepository _timeSheetReadOnlyRepository;
        private readonly IHolidayWriteOnlyRepository _holidayWriteOnlyRepository;
        private readonly IBatchUpdateTimeSheetClockingsDomainService _batchUpdateTimeSheetClockingsDomainService;

        public DeleteHolidayCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            ITimeSheetReadOnlyRepository timeSheetReadOnlyRepository,
            IHolidayWriteOnlyRepository holidayWriteOnlyRepository,
            IBatchUpdateTimeSheetClockingsDomainService batchUpdateTimeSheetClockingsDomainService

        ) : base(eventDispatcher)
        {
            _mapper = mapper;
            _eventDispatcher = eventDispatcher;
            _timeSheetReadOnlyRepository = timeSheetReadOnlyRepository;
            _holidayWriteOnlyRepository = holidayWriteOnlyRepository;
            _batchUpdateTimeSheetClockingsDomainService = batchUpdateTimeSheetClockingsDomainService;
        }

        public async Task<HolidayDto> Handle(DeleteHolidayCommand request, CancellationToken cancellationToken)
        {
            var holiday = await _holidayWriteOnlyRepository.FindByIdAsync(request.Id);

            if (holiday == null)
            {
                await _eventDispatcher.FireEvent(new DomainNotification(typeof(Holiday).Name, Message.holiday_notExist));
                return null;
            }

            var timeSheetsNeedAdd = await _timeSheetReadOnlyRepository.GetTimeSheetOverlayHoliday(holiday.From, holiday.To);
            if (!request.IsOverLapClocking && timeSheetsNeedAdd.Any())
            {
                return null;
            }

            if (timeSheetsNeedAdd.Any() && request.IsAddClocking)
            {
                // Lấy danh sách lễ tết trong chi nhánh
                var holidayOfCurrentBranch = await _holidayWriteOnlyRepository.GetBySpecificationAsync(
                    new DefaultTrueSpec<Holiday>()
                        .Not(new FindHolidayByIdSpec(holiday.Id)));
                var generateClockingForTimeSheetsDto = new GenerateClockingForTimeSheetsDto()
                {
                    TimeSheets = timeSheetsNeedAdd,
                    IsAddClockings = true,
                    IsRemoveClockings = false,
                    ApplyTimes = new List<DateRangeDto>()
                    {
                        new DateRangeDto()
                        {
                            From = holiday.From,
                            To = holiday.To
                        }
                    },
                    Holidays = holidayOfCurrentBranch,
                    GenerateByType = GenerateClockingByType.Holiday
                };
                // Thực hiện bổ sung chi tiết làm việc cho lịch làm việc có thời gian giao khoảng thời gian nghỉ lễ bị xóa
                var resultUpdate = await _batchUpdateTimeSheetClockingsDomainService.BatchUpdateTimeSheetClockingWhenUpdateDaysOffAsync(generateClockingForTimeSheetsDto);

                if (!resultUpdate.IsValid)
                {
                    NotifyValidationErrors(typeof(TimeSheet), resultUpdate.ValidationErrors);
                    return null;
                }
            }

            holiday.Delete();
            _holidayWriteOnlyRepository.Delete(holiday);

            await _holidayWriteOnlyRepository.UnitOfWork.CommitAsync();

            var result = _mapper.Map<HolidayDto>(holiday);
            return result;
        }
    }
}
