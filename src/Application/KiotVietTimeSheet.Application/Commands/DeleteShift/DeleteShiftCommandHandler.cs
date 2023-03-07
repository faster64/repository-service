using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.DeleteShift
{
    public class DeleteShiftCommandHandler : BaseCommandHandler,
        IRequestHandler<DeleteShiftCommand, ShiftDto>
    {
        private readonly IMapper _mapper;
        private readonly IShiftWriteOnlyRepository _shiftWriteOnlyRepository;
        private readonly IClockingReadOnlyRepository _clockingRepository;
        private readonly IEventDispatcher _eventDispatcher;

        public DeleteShiftCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IShiftWriteOnlyRepository shiftWriteOnlyRepository,
            IClockingReadOnlyRepository clockingRepository
        )
            : base(eventDispatcher)
        {
            _mapper = mapper;
            _shiftWriteOnlyRepository = shiftWriteOnlyRepository;
            _clockingRepository = clockingRepository;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<ShiftDto> Handle(DeleteShiftCommand request, CancellationToken cancellationToken)
        {
            var oldShiftItem = await _shiftWriteOnlyRepository.FindByIdAsync(request.Id);

            if (oldShiftItem == null)
            {
                NotifyShiftInDbIsNotExists();
                return null;
            }

            oldShiftItem.Delete();

            var findClockingByShiftIdSpec = (new FindClockingByShiftIdSpec(oldShiftItem.Id)).Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void));
            if (await _clockingRepository.AnyBySpecificationAsync(findClockingByShiftIdSpec))
            {
                await _eventDispatcher.FireEvent(new DomainNotification(typeof(Shift).Name, Message.shift_cannotDelete));
                return null;
            }

            _shiftWriteOnlyRepository.Delete(oldShiftItem);

            await _shiftWriteOnlyRepository.UnitOfWork.CommitAsync();

            var result = _mapper.Map<ShiftDto>(oldShiftItem);
            return result;
        }

        private void NotifyShiftInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(Shift).Name, string.Format(Message.not_exists, Label.shift)));
        }
    }
}
