using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.ShiftValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UpdateShift
{
    public class UpdateShiftCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateShiftCommand, ShiftDto>
    {
        private readonly IShiftWriteOnlyRepository _shiftWriteOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IRejectClockingsDomainService _rejectClockingsService;
        private readonly IMapper _mapPer;
        private readonly IEventDispatcher _eventDispatcher;

        public UpdateShiftCommandHandler(
            IMapper mapper,
            IEventDispatcher eventDispatcher,
            IShiftWriteOnlyRepository shiftWriteOnlyRepository,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IRejectClockingsDomainService rejectClockingsService
        )
            : base(eventDispatcher)
        {
            _shiftWriteOnlyRepository = shiftWriteOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _rejectClockingsService = rejectClockingsService;
            _mapPer = mapper;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<ShiftDto> Handle(UpdateShiftCommand request, CancellationToken cancellationToken)
        {
            var shiftDto = request.Shift;
            var shift = await _shiftWriteOnlyRepository.FindByIdAsync(shiftDto.Id);
            if (shift == null)
            {
                NotifyShiftInDbIsNotExists();
                return null;
            }

            if (!shiftDto.IsActive)
            {
                var clockings = await _clockingReadOnlyRepository.GetBySpecificationAsync(new FindClockingByShiftIdSpec(shift.Id).And(new FindClockingByStatusSpec((byte)ClockingStatuses.Created)));
                var returnObj = await _rejectClockingsService.RejectClockingsAsync(clockings);
                if (!returnObj)
                {
                    return null;
                }
            }
            
            var from = shift.From;
            var to = shift.To;
            shift.Update(shiftDto.Name, shiftDto.From, shiftDto.To, shiftDto.IsActive, shiftDto.CheckInBefore, shiftDto.CheckOutAfter, request.IsGeneralSetting);

            var validateResult = await (new CreateOrUpdateShiftAsyncValidator(_shiftReadOnlyRepository, shift)).ValidateAsync(shift);

            if (!validateResult.IsValid)
            {
                NotifyValidationErrors(typeof(Holiday), validateResult.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }

            _shiftWriteOnlyRepository.Update(shift);

            await _shiftWriteOnlyRepository.UnitOfWork.CommitAsync();

            var result = _mapPer.Map<ShiftDto>(shift);
            result.OldFrom = from;
            result.OldTo = to;
            return result;
        }

        private void NotifyShiftInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(Shift).Name, string.Format(Message.not_exists, Label.shift)));
        }
    }
}
