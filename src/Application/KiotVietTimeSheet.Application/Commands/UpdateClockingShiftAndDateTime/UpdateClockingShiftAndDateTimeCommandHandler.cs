using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.ClockingValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Commands.UpdateClockingShiftAndDateTime
{
    public class UpdateClockingShiftAndDateTimeCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateClockingShiftAndDateTimeCommand, ClockingDto>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IMapper _mapper;
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;

        public UpdateClockingShiftAndDateTimeCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService 
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _mapper = mapper;
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task<ClockingDto> Handle(UpdateClockingShiftAndDateTimeCommand request, CancellationToken cancellationToken)
        {
            var clockingId = request.ClockingId;
            var shiftTargetId = request.ShiftTargetId;
            var workingDay = request.WorkingDay;
            var oldClocking = await _clockingWriteOnlyRepository.FindBySpecificationAsync(new FindClockingByClockingIdSpec(clockingId)
                .Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void)));
            var shiftTarget = await _shiftReadOnlyRepository.FindByIdAsync(shiftTargetId);

            if (oldClocking == null)
            {
                NotifyClockingInDbIsNotExists();
                return null;
            }

            var newClocking = oldClocking.CreateCopy();
            if (shiftTarget != null)
            {
                newClocking.UpdateClockingShiftAndDateTime(shiftTargetId, workingDay, shiftTarget);
            }

            var validateResult = await (new UpdateClockingShiftAndDateTimeValidator(_clockingReadOnlyRepository, _shiftReadOnlyRepository, oldClocking.ShiftId, shiftTargetId, newClocking.StartTime, newClocking.EndTime)).ValidateAsync(newClocking);
            if (!validateResult.IsValid)
            {
                NotifyValidationErrors(typeof(Clocking), validateResult.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }

            _clockingWriteOnlyRepository.Update(newClocking);

            await _clockingWriteOnlyRepository.UnitOfWork.CommitAsync(false);

            var listClockingForIntegrationEvent = new List<Tuple<Clocking, Clocking>> { new Tuple<Clocking, Clocking>(oldClocking, newClocking) };
            await _timeSheetIntegrationEventService.AddEventAsync(new UpdateMultipleClockingIntegrationEvent(listClockingForIntegrationEvent));

            var result = _mapper.Map<ClockingDto>(newClocking);
            return result;
        }
        private void NotifyClockingInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(Clocking).Name, Message.clocking_notExist));
        }
    }
}
