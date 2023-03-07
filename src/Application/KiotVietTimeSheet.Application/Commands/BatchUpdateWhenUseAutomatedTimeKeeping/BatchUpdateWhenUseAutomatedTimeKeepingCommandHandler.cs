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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Queries.GetSetting;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Validators;

namespace KiotVietTimeSheet.Application.Commands.BatchUpdateWhenUseAutomatedTimeKeeping
{
    public class BatchUpdateWhenUseAutomatedTimeKeepingCommandHandler : BaseCommandHandler,
        IRequestHandler<BatchUpdateWhenUseAutomatedTimeKeepingCommand, List<ClockingDto>>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IMapper _mapper;
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly IClockingHistoryWriteOnlyRepository _clockingHistoryWriteOnlyRepository;
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;

        public BatchUpdateWhenUseAutomatedTimeKeepingCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            IClockingHistoryWriteOnlyRepository clockingHistoryWriteOnlyRepository,
            IMediator mediator,
            IAuthService authService
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _mapper = mapper;
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _clockingHistoryWriteOnlyRepository = clockingHistoryWriteOnlyRepository;
            _mediator = mediator;
            _authService = authService;
        }

        public async Task<List<ClockingDto>> Handle(BatchUpdateWhenUseAutomatedTimeKeepingCommand request, CancellationToken cancellationToken)
        {
            var automatedTimekeepingDtos = request.AutomatedTimekeepingDtos;
            // Code tạm để test, đợi BA ra srs
            var clockingSpecification = new FindClockingByEmployeeIdsSpec(automatedTimekeepingDtos.Select(x => x.EmployeeId).ToList())
                .Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void));
            var listClockingByEmployeeId = await _clockingReadOnlyRepository.GetBySpecificationAsync(clockingSpecification, true);
            var oldListClocking = new List<Clocking>();

            var settingsBatchUpdateCommand = await _mediator.Send(new GetSettingQuery(_authService.Context.TenantId), cancellationToken);

            var clockingHistoriesBatchUpdateCommand = new List<ClockingHistory>();
            foreach (var item in automatedTimekeepingDtos)
            {
                var specificationClockingQueryAble = new FindClockingByEmployeeIdSpec(item.EmployeeId)
                    .And(new FindClockingByStartTimeLessThanOrEqualSpec(item.TimeKeepingDate))
                    .And(new FindClockingByEndTimeGreaterThanOrEqualSpec(item.TimeKeepingDate));

                var clockingFromSource = listClockingByEmployeeId.FirstOrDefault(specificationClockingQueryAble.GetExpression().Compile());

                if(clockingFromSource == null) continue;
                
                var shift = await _shiftReadOnlyRepository.FindByIdAsync(clockingFromSource.ShiftId);
                var lastClockingHistory = clockingFromSource.ClockingHistories?.OrderByDescending(ch => ch.CreatedDate).FirstOrDefault();
                DateTime? checkedInDate = item.TimeKeepingDate;
                DateTime? checkedOutDate = null;
                if (lastClockingHistory != null && lastClockingHistory.ClockingStatus == (byte)ClockingStatuses.CheckedIn)
                {
                    checkedInDate = lastClockingHistory.CheckedInDate;
                    checkedOutDate = item.TimeKeepingDate;
                }

                var newClockingHistory = ClockingHistory.GenerateClockingHistory(
                    clockingFromSource.Id,
                    clockingFromSource.BranchId,
                    clockingFromSource.TenantId,
                    checkedInDate,
                    checkedOutDate,
                    clockingFromSource.StartTime,
                    clockingFromSource.EndTime,
                    (byte)TimeKeepingTypes.Fingerprint,
                    settingsBatchUpdateCommand,
                    clockingFromSource.ClockingStatus,
                    int.MaxValue,
                    int.MaxValue,
                    int.MaxValue,
                    int.MaxValue,
                    clockingFromSource.AbsenceType,
                    null,
                    false,
                    shift.Id,
                    clockingFromSource.EmployeeId,
                    lastClockingHistory
                );

                if (newClockingHistory != null)
                {
                    var validator = await (new CreateOrUpdateClockingHistoryValidator(clockingFromSource.StartTime, clockingFromSource.EndTime)).ValidateAsync(newClockingHistory);
                    if (!validator.IsValid)
                    {
                        NotifyValidationErrors(typeof(ClockingHistory), validator.Errors.Select(e => e.ErrorMessage).ToList());
                        return null;
                    }

                    clockingHistoriesBatchUpdateCommand.Add(newClockingHistory);
                }

                oldListClocking.Add(clockingFromSource);
                
            }

            if (!oldListClocking.Any())
            {
                NotifyClockingInDbIsNotExists();
                return null;
            }

            _clockingHistoryWriteOnlyRepository.BatchAdd(clockingHistoriesBatchUpdateCommand);

            var newListClocking = new List<Clocking>();
            foreach (var clocking in oldListClocking)
            {
                var clockingHistoryByClockingId = clockingHistoriesBatchUpdateCommand.FirstOrDefault(ch => ch.ClockingId == clocking.Id);
                var newClocking = GetNewClocking(clocking, clockingHistoryByClockingId);

                newListClocking.Add(newClocking);
            }

            _clockingWriteOnlyRepository.BatchUpdate(newListClocking);
            await _clockingWriteOnlyRepository.UnitOfWork.CommitAsync();
            var result = _mapper.Map<List<ClockingDto>>(newListClocking);
            return result;
        }

        public Clocking GetNewClocking(Clocking clocking, ClockingHistory clockingHistoryByClockingId)
        {
            var newClocking = clocking.CreateCopy();

            if (clockingHistoryByClockingId != null)
                newClocking.UpdateClockingAfterCreateClockingHistory(clockingHistoryByClockingId, clocking);

            return newClocking;
        }
        private void NotifyClockingInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(nameof(Clocking), Message.clocking_notExist));
        }
    }
}

