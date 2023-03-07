using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
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
using KiotVietTimeSheet.Application.Queries.GetSetting;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Validators;

namespace KiotVietTimeSheet.Application.Commands.BatchUpdateClocking
{
    public class BatchUpdateClockingCommandHandler : BaseCommandHandler,
        IRequestHandler<BatchUpdateClockingCommand, List<ClockingDto>>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IMapper _mapper;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly IClockingHistoryWriteOnlyRepository _clockingHistoryWriteOnlyRepository;
        private readonly IPaySheetOutOfDateDomainService _paySheetOutOfDateDomainService;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IAuthService _authService;
        private readonly IMediator _mediator;

        public BatchUpdateClockingCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            IAuthService authService,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            IClockingHistoryWriteOnlyRepository clockingHistoryWriteOnlyRepository,
            IPaySheetOutOfDateDomainService paySheetOutOfDateDomainService,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IKiotVietServiceClient kiotVietServiceClient,
            IMediator mediator
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _mapper = mapper;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _clockingHistoryWriteOnlyRepository = clockingHistoryWriteOnlyRepository;
            _paySheetOutOfDateDomainService = paySheetOutOfDateDomainService;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _kiotVietServiceClient = kiotVietServiceClient;
            _authService = authService;
            _mediator = mediator;
        }

        public async Task<Tuple<string, bool>> GetHavingPermission(string oldBranchName, List<ClockingDto> clockingDtos)
        {
            var branchName = oldBranchName;
            var isHavePermission = true;
            foreach (var clocking in clockingDtos)
            {
                isHavePermission = await IsHavePermissionOnBranch(clocking.BranchId, TimeSheetPermission.Clocking_Update, _authService.Context.User.IsAdmin);
                if (isHavePermission) continue;
                var branch = await _kiotVietServiceClient.GetBranchById(clocking.BranchId);
                if (branch != null)
                    branchName = branch.Name;
                break;
            }
            return new Tuple<string, bool>(branchName, isHavePermission);
        }
        public async Task<List<ClockingDto>> Handle(BatchUpdateClockingCommand request, CancellationToken cancellationToken)
        {
            var clockingDtos = request.ClockingDtos;
            var clockingHistoryDto = request.ClockingHistoryDto;
            var leaveOfAbsence = request.LeaveOfAbsence;
            bool isHavePermission = true;
            string branchName = string.Empty;

            var tuplePermission = await GetHavingPermission(branchName, clockingDtos);
            branchName = tuplePermission.Item1;
            isHavePermission = tuplePermission.Item2;

            var errors = new List<string>();
            if (!isHavePermission)
            {
                string message = string.Format(Message.clocking_updateError, branchName);
                errors.Add(message);
                NotifyValidationErrors(typeof(Clocking), errors);
                return null;
            }

            var oldClockings = await _clockingWriteOnlyRepository.GetBySpecificationAsync(new FindClockingByIdsSpec(clockingDtos.Select(c => c.Id).ToList())
                .Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void)), "ClockingHistories");
            if (!oldClockings.Any())
            {
                await _eventDispatcher.FireEvent(new DomainNotification(nameof(Clocking), Message.clocking_allClockingsDoNotExist));
                return null;
            }

            var validator = await (new BatchUpdateClockingStatusAndBatchAddClockingHistoriesValidator(oldClockings).ValidateAsync(oldClockings[0]));
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(Clocking), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }
            var shift = await _shiftReadOnlyRepository.FindByIdAsync(oldClockings.First().ShiftId);

            var settingObjectDto = await _mediator.Send(new GetSettingQuery(_authService.Context.TenantId), cancellationToken);

            var clockingHistories = new List<ClockingHistory>();
            foreach (var clocking in oldClockings)
            {
                var newClockingHistory = ClockingHistory.GenerateClockingHistory(
                    clocking.Id,
                    clocking.BranchId,
                    clocking.TenantId,
                    clockingHistoryDto.CheckedInDate,
                    clockingHistoryDto.CheckedOutDate,
                    clocking.StartTime,
                    clocking.EndTime,
                    (byte)TimeKeepingTypes.Manual,
                    settingObjectDto,
                    clocking.ClockingStatus,
                    clockingHistoryDto.TimeIsLate,
                    clockingHistoryDto.OverTimeBeforeShiftWork,
                    clockingHistoryDto.TimeIsLeaveWorkEarly,
                    clockingHistoryDto.OverTimeAfterShiftWork,
                    clocking.AbsenceType,
                    clockingHistoryDto.AbsenceType,
                    leaveOfAbsence,
                    shift.Id,
                    clocking.EmployeeId,
                    clocking.ClockingHistories?.OrderByDescending(ch => ch.CreatedDate).FirstOrDefault(),
                    null,
                    null,
                    clockingHistoryDto.CheckInDateType,
                    clockingHistoryDto.CheckOutDateType
                );
                if(newClockingHistory == null)
                    continue;
                validator = await (new CreateOrUpdateClockingHistoryValidator(clocking.StartTime, clocking.EndTime)).ValidateAsync(newClockingHistory);
                if (!validator.IsValid)
                {
                    NotifyValidationErrors(typeof(ClockingHistory), validator.Errors.Select(e => e.ErrorMessage).ToList());
                    return null;
                }

                clockingHistories.Add(newClockingHistory);
            }
            _clockingHistoryWriteOnlyRepository.BatchAdd(clockingHistories);

            var listClockingHaveChangeSalaryData = new List<Clocking>();
            var newClockings = new List<Clocking>();
            var listClockingForIntegrationEvent = new List<Tuple<Clocking, Clocking>>();

            foreach (var clocking in oldClockings)
            {
                var clockingHistoryByClockingId = clockingHistories.FirstOrDefault(ch => ch.ClockingId == clocking.Id);
                var newClocking = clocking.CreateCopy();

                if (clockingHistoryByClockingId != null)
                    newClocking.UpdateClockingAfterCreateClockingHistory(clockingHistoryByClockingId, clocking);

                if (!newClocking.IsDataNotChanged(clocking, false))
                    listClockingHaveChangeSalaryData.Add(newClocking);

                newClockings.Add(newClocking);
                listClockingForIntegrationEvent.Add(new Tuple<Clocking, Clocking>(clocking, newClocking));
            }

            await _paySheetOutOfDateDomainService.WithClockingDataChangeAsync(listClockingHaveChangeSalaryData
                .Where(x => x.ClockingPaymentStatus != (byte)ClockingPaymentStatuses.Paid).ToList());

            _clockingWriteOnlyRepository.BatchUpdate(newClockings);
            await _timeSheetIntegrationEventService.AddEventAsync(new UpdateMultipleClockingIntegrationEvent(listClockingForIntegrationEvent));
            await _clockingWriteOnlyRepository.UnitOfWork.CommitAsync();
            var result = _mapper.Map<List<ClockingDto>>(newClockings);
            return result;
        }

        private async Task<bool> IsHavePermissionOnBranch(int branchId, string permissionName, bool isAdmin)
        {
            if (isAdmin)
                return true;
            var permission = await _kiotVietServiceClient.GetPermissionByBranchId(_authService.Context.User.Id, branchId);
            return permission.Data.ContainsKey(permissionName) && permission.Data[permissionName];
        }
    }
}
