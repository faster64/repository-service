using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Queries.GetSetting;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Validators;

namespace KiotVietTimeSheet.Application.Commands.UpdateWhenUseAutomatedTimeKeeping
{
    public class UpdateWhenUseAutomatedTimeKeepingCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateWhenUseAutomatedTimeKeepingCommand, ClockingDto>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IMapper _mapper;
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly IClockingHistoryWriteOnlyRepository _clockingHistoryWriteOnlyRepository;
        private readonly IAuthService _authService;
        private readonly IMediator _mediator;

        public UpdateWhenUseAutomatedTimeKeepingCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            IClockingHistoryWriteOnlyRepository clockingHistoryWriteOnlyRepository,
            IAuthService authService,
            IMediator mediator
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _mapper = mapper;
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _clockingHistoryWriteOnlyRepository = clockingHistoryWriteOnlyRepository;
            _authService = authService;
            _mediator = mediator;
        }

        public async Task<ClockingDto> Handle(UpdateWhenUseAutomatedTimeKeepingCommand request, CancellationToken cancellationToken)
        {
            var timeKeepingDate = request.TimeKeepingDate;
            var employeeId = request.EmployeeId;

            // Code tạm để test, đợi BA ra srs
            var clockingSpecification = new FindClockingByEmployeeIdSpec(employeeId)
                .Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void))
                .And(new FindClockingByStartTimeLessThanOrEqualSpec(timeKeepingDate))
                .And(new FindClockingByEndTimeGreaterThanOrEqualSpec(timeKeepingDate));
            var oldClocking = await _clockingReadOnlyRepository.FindBySpecificationAsync(clockingSpecification, true);

            if (oldClocking == null)
            {
                NotifyClockingInDbIsNotExists();
                return null;
            }

            var newClocking = oldClocking.CreateCopy();
            var lastClockingHistory = oldClocking.ClockingHistories?.OrderByDescending(ch => ch.CreatedDate).FirstOrDefault();
            DateTime? checkedInDate = timeKeepingDate;
            DateTime? checkedOutDate = null;

            if (lastClockingHistory != null && lastClockingHistory.ClockingStatus == (byte)ClockingStatuses.CheckedIn)
            {
                    checkedInDate = lastClockingHistory.CheckedInDate;
                    checkedOutDate = timeKeepingDate;
            }
            var settings = await _mediator.Send(new GetSettingQuery(_authService.Context.TenantId), cancellationToken);
            var newClockingHistory = ClockingHistory.GenerateClockingHistory(
                newClocking.Id,
                newClocking.BranchId,
                newClocking.TenantId,
                checkedInDate,
                checkedOutDate,
                newClocking.StartTime,
                newClocking.EndTime,
                (byte)TimeKeepingTypes.Fingerprint,
                settings,
                newClocking.ClockingStatus,
                int.MaxValue,
                int.MaxValue,
                int.MaxValue,
                int.MaxValue,
                oldClocking.AbsenceType,
                null,
                false,
                newClocking.ShiftId,
                newClocking.EmployeeId,
                lastClockingHistory
            );

            newClocking.UpdateClockingAfterCreateClockingHistory(newClockingHistory, oldClocking);

            if (newClockingHistory != null)
            {
                var validateResult = await (new CreateOrUpdateClockingHistoryValidator(newClocking.StartTime, newClocking.EndTime)).ValidateAsync(newClockingHistory);
                if (!validateResult.IsValid)
                {
                    NotifyValidationErrors(typeof(ClockingHistory), validateResult.Errors.Select(e => e.ErrorMessage).ToList());
                    return null;
                }
                _clockingHistoryWriteOnlyRepository.Add(newClockingHistory);
            }

            _clockingWriteOnlyRepository.Update(newClocking);
            await _clockingWriteOnlyRepository.UnitOfWork.CommitAsync();
            var result = _mapper.Map<ClockingDto>(newClocking);
            return result;
        }
        private void NotifyClockingInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(Clocking).Name, Message.clocking_notExist));
        }
    }
}

