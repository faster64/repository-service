using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.HolidayValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CreateHoliday
{
    public class CreateHolidayCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateHolidayCommand, HolidayDto>
    {
        private readonly IMapper _mapper;
        private readonly CreateOrUpdateHolidayAsyncValidator _createOrUpdateHolidayAsyncValidator;
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IHolidayWriteOnlyRepository _holidayWriteOnlyRepository;
        private readonly IRejectClockingsDomainService _rejectClockingsDomainService;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        public CreateHolidayCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            CreateOrUpdateHolidayAsyncValidator createOrUpdateHolidayAsyncValidator,
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IHolidayWriteOnlyRepository holidayWriteOnlyRepository,
            IRejectClockingsDomainService rejectClockingsDomainService,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService)
            : base(eventDispatcher)
        {
            _mapper = mapper;
            _createOrUpdateHolidayAsyncValidator = createOrUpdateHolidayAsyncValidator;
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _holidayWriteOnlyRepository = holidayWriteOnlyRepository;
            _rejectClockingsDomainService = rejectClockingsDomainService;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task<HolidayDto> Handle(CreateHolidayCommand request, CancellationToken cancellationToken)
        {
            var holidayDto = request.HolidayDto;
            holidayDto.From = holidayDto.From.Date;
            holidayDto.To = holidayDto.To.Date;

            var holiday = new Holiday(holidayDto.Name, holidayDto.From, holidayDto.To);

            var validateResult = await _createOrUpdateHolidayAsyncValidator.ValidateAsync(holiday, cancellationToken);

            if (!validateResult.IsValid)
            {
                NotifyValidationErrors(typeof(Holiday), validateResult.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }

            var clockingsNeedCancel = await _clockingReadOnlyRepository.GetClockingOverlapHoliday(holiday.From, holiday.To);
            if (!request.IsShowPopupOverLapClocking && clockingsNeedCancel.Any())
            {
                return null;
            }

            _holidayWriteOnlyRepository.Add(holiday);

            if (request.IsCancelClocking)
            {
                var rejectResult = await _rejectClockingsDomainService.RejectClockingsAsync(clockingsNeedCancel);
                if (!rejectResult)
                {
                    return null;
                }

                await _timeSheetIntegrationEventService.AddEventAsync(new RejectMultipleClockingIntegrationEvent(clockingsNeedCancel));
            }
            
            await _holidayWriteOnlyRepository.UnitOfWork.CommitAsync();

            var result = _mapper.Map<HolidayDto>(holiday);
            return result;
        }
    }
}
