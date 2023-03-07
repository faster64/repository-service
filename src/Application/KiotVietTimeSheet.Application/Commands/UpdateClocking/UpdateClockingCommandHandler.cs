using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.Validators.ClockingValidators;
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
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using ServiceStack;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Validators;

namespace KiotVietTimeSheet.Application.Commands.UpdateClocking
{
    public class UpdateClockingCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateClockingCommand, ClockingDto>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IMapper _mapper;
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly IClockingHistoryWriteOnlyRepository _clockingHistoryWriteOnlyRepository;
        private readonly ICreateTimeSheetClockingDomainService _createTimeSheetClockingDomainService;
        private readonly IPaySheetOutOfDateDomainService _paySheetOutOfDateDomainService;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IAuthService _authService;
        private readonly IClockingPenalizeWriteOnlyRepository _clockingPenalizeWriteOnlyRepository;
        private readonly IMediator _mediator;

        public UpdateClockingCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            IAuthService authService,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            IClockingHistoryWriteOnlyRepository clockingHistoryWriteOnlyRepository,
            ICreateTimeSheetClockingDomainService createTimeSheetClockingDomainService,
            IPaySheetOutOfDateDomainService paySheetOutOfDateDomainService,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IKiotVietServiceClient kiotVietServiceClient,
            IClockingPenalizeWriteOnlyRepository clockingPenalizeWriteOnlyRepository,
            IMediator mediator
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _mapper = mapper;
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _clockingHistoryWriteOnlyRepository = clockingHistoryWriteOnlyRepository;
            _createTimeSheetClockingDomainService = createTimeSheetClockingDomainService;
            _paySheetOutOfDateDomainService = paySheetOutOfDateDomainService;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _kiotVietServiceClient = kiotVietServiceClient;
            _authService = authService;
            _mediator = mediator;
            _clockingPenalizeWriteOnlyRepository = clockingPenalizeWriteOnlyRepository;
        }

        public async Task<ClockingDto> Handle(UpdateClockingCommand request, CancellationToken cancellationToken)
        {
            var clockingDto = request.ClockingDto;
            var replacementEmployeeId = request.ReplacementEmployeeId;
            var clockingHistoryDto = request.ClockingHistoryDto;
            var leaveOfAbsence = request.LeaveOfAbsence;
            var isHavePermission = await IsHavePermissionUpdateClockingOnBranch(clockingDto.BranchId, TimeSheetPermission.Clocking_Update, _authService.Context.User.IsAdmin);
            if (!isHavePermission)
            {
                var branch = await _kiotVietServiceClient.GetBranchById(clockingDto.BranchId);
                var branchName = branch?.Name ?? string.Empty;
                var message = string.Format(Message.clocking_updateError, branchName);
                NotifyValidationErrors(typeof(Clocking), new List<string> { message });
                return null;
            }

            var includes = new List<string>
            {
                nameof(Clocking.ClockingHistories),
                nameof(Clocking.ClockingPenalizes)
            };

            var oldClocking = await _clockingWriteOnlyRepository.FindBySpecificationWithIncludesAsync(new FindClockingByClockingIdSpec(clockingDto.Id)
                .Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void)), includes);

            var shiftTarget = await _shiftReadOnlyRepository.FindByIdAsync(clockingDto.ShiftId);

            if (oldClocking == null)
            {
                NotifyUpdateClockingInDbIsNotExists();
                return null;
            }

            var newClockingItem = oldClocking.CreateCopy();
            if (shiftTarget != null)
            {
                newClockingItem.UpdateWithoutAddDomainEvent(
                    clockingDto.ShiftId,
                    clockingDto.StartTime,
                    clockingDto.EndTime,
                    clockingDto.Note,
                    shiftTarget
                );
            }

            var validateResult = await (new UpdateClockingValidator(_clockingReadOnlyRepository, _shiftReadOnlyRepository, oldClocking.ShiftId, clockingDto.ShiftId, newClockingItem.StartTime, newClockingItem.EndTime)).ValidateAsync(newClockingItem);
            if (!validateResult.IsValid)
            {
                NotifyValidationErrors(typeof(Clocking), validateResult.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }

            var settingsObjectDto = await _mediator.Send(new GetSettingQuery(_authService.Context.TenantId), cancellationToken);

            if (shiftTarget != null)
            {
                var newClockingHistoryItem = ClockingHistory.GenerateClockingHistory(
                    newClockingItem.Id,
                    newClockingItem.BranchId,
                    newClockingItem.TenantId,
                    clockingHistoryDto.CheckedInDate,
                    clockingHistoryDto.CheckedOutDate,
                    newClockingItem.StartTime,
                    newClockingItem.EndTime,
                    (byte)TimeKeepingTypes.Manual,
                    settingsObjectDto,
                    newClockingItem.ClockingStatus,
                    clockingHistoryDto.TimeIsLate,
                    clockingHistoryDto.OverTimeBeforeShiftWork,
                    clockingHistoryDto.TimeIsLeaveWorkEarly,
                    clockingHistoryDto.OverTimeAfterShiftWork,
                    oldClocking.AbsenceType,
                    clockingHistoryDto.AbsenceType,
                    leaveOfAbsence,
                    shiftTarget.Id,
                    newClockingItem.EmployeeId,
                    newClockingItem.ClockingHistories?.OrderByDescending(ch => ch.CreatedDate).FirstOrDefault(),
                    null,
                    null,
                    clockingHistoryDto.CheckInDateType,
                    clockingHistoryDto.CheckOutDateType
                );

                newClockingItem.UpdateClockingAfterCreateClockingHistory(newClockingHistoryItem, oldClocking);

                if (newClockingHistoryItem != null)
                {
                    validateResult = await (new CreateOrUpdateClockingHistoryValidator(newClockingItem.StartTime, newClockingItem.EndTime)).ValidateAsync(newClockingHistoryItem);
                    if (!validateResult.IsValid)
                    {
                        NotifyValidationErrors(typeof(ClockingHistory), validateResult.Errors.Select(e => e.ErrorMessage).ToList());
                        return null;
                    }
                    _clockingHistoryWriteOnlyRepository.Add(newClockingHistoryItem);
                }
            }

            _clockingWriteOnlyRepository.Update(newClockingItem);

           await UpdateClockingPenalize(request.ClockingDto.ClockingPenalizesDto, oldClocking.ClockingPenalizes, newClockingItem, oldClocking.EmployeeId);

            if (!oldClocking.IsDataNotChanged(newClockingItem, leaveOfAbsence) && oldClocking.ClockingPaymentStatus != (byte)ClockingPaymentStatuses.Paid)
            {
                await _paySheetOutOfDateDomainService.WithClockingDataChangeAsync(new List<Clocking> { newClockingItem });
            }

            var isValidClockingDto = await CheckValidClockingDto(leaveOfAbsence, replacementEmployeeId, clockingDto);

            if (!isValidClockingDto)
            {
                return null;
            }

            var listClockingForIntegrationEvent = new List<Tuple<Clocking, Clocking>> { new Tuple<Clocking, Clocking>(oldClocking, newClockingItem) };

            await _clockingWriteOnlyRepository.UnitOfWork.CommitAsync(false);
            await _timeSheetIntegrationEventService.AddEventAsync(new UpdateMultipleClockingIntegrationEvent(listClockingForIntegrationEvent));

            var result = _mapper.Map<ClockingDto>(newClockingItem);
            return result;
        }

        private async Task UpdateClockingPenalize(List<ClockingPenalizeDto> newClockingPenalizesDto, List<ClockingPenalize> oldClockingPenalizes, Clocking newClockingItem, long employeeId)
        {
            if (newClockingPenalizesDto == null && oldClockingPenalizes == null)
            {
                return;
            }

            if (newClockingPenalizesDto == null)
            {
                newClockingPenalizesDto = new List<ClockingPenalizeDto>();
            }

            if (oldClockingPenalizes == null)
            {
                oldClockingPenalizes = new List<ClockingPenalize>();
            }

            var clockingPenalizeAfterUpdate = new List<ClockingPenalize>();

            var listNewClockingPenalizeIds = newClockingPenalizesDto.Select(x => x.Id).ToList();

            var cancelClockingPenalize =
                oldClockingPenalizes.Where(x => !listNewClockingPenalizeIds.Contains(x.Id)).ToList();
            if (cancelClockingPenalize.Any())
            {
                _clockingPenalizeWriteOnlyRepository.BatchDelete(cancelClockingPenalize);
            }

            var newClockingPenalizeDto = newClockingPenalizesDto.Where(x => x.Id == 0).ToList();
            if (newClockingPenalizeDto.Any())
            {
                foreach (var clockingPenalize in newClockingPenalizeDto)
                {
                    clockingPenalize.EmployeeId = employeeId;
                }

                var clockingPenalizeMap = _mapper.Map<List<ClockingPenalize>>(newClockingPenalizeDto);
                clockingPenalizeAfterUpdate.AddRange(clockingPenalizeMap);
                _clockingPenalizeWriteOnlyRepository.BatchAdd(clockingPenalizeMap);
            }

            var updateClockingPenalize = oldClockingPenalizes.Where(x => listNewClockingPenalizeIds.Contains(x.Id)).ToList();
            var oldClockingId = updateClockingPenalize.Select(x => x.ClockingId).FirstOrDefault();
            var lstClockingPenalizeIdsUpdate = updateClockingPenalize.Select(x => x.PenalizeId).ToList();
            var lstClockingPenalize = new List<ClockingPenalize>();
            if (oldClockingId > 0)
            {
                lstClockingPenalize = await _clockingPenalizeWriteOnlyRepository.GetBySpecificationAsync(
                    new FindClockingPenalizeByPenalizeIdsSpec(lstClockingPenalizeIdsUpdate).And(
                        new FindClockingPenalizeByClockingIdSpec(oldClockingId)));
            }

            lstClockingPenalize.ForEach(clockingPenalize =>
            {
                var clockingPenalizeDto = newClockingPenalizesDto.FirstOrDefault(x => x.Id == clockingPenalize.Id);
                if (clockingPenalizeDto == null)
                {
                    return;
                }

                clockingPenalize.Update(clockingPenalizeDto.TimesCount, clockingPenalizeDto.Value, clockingPenalizeDto.PenalizeId);
            });

            if (lstClockingPenalize.Any())
            {
                _clockingPenalizeWriteOnlyRepository.BatchUpdate(lstClockingPenalize);
                clockingPenalizeAfterUpdate.AddRange(lstClockingPenalize);
            }

            newClockingItem.ClockingPenalizes = clockingPenalizeAfterUpdate;
        }

        private async Task<bool> CheckValidClockingDto(bool leaveOfAbsence, long replacementEmployeeId, ClockingDto clockingDto)
        {
            if (replacementEmployeeId <= 0 || !leaveOfAbsence) return true;

            var replacementClockingDtoItem = clockingDto.CreateCopy();
            replacementClockingDtoItem.EmployeeId = replacementEmployeeId;

            var timeSheetDto = new TimeSheetDto
            {
                EmployeeId = replacementClockingDtoItem.EmployeeId,
                StartDate = replacementClockingDtoItem.StartTime,
                IsRepeat = false,
                RepeatType = null,
                RepeatEachDay = null,
                EndDate = replacementClockingDtoItem.EndTime,
                BranchId = replacementClockingDtoItem.BranchId,
                SaveOnDaysOffOfBranch = false,
                SaveOnHoliday = false,
                TimeSheetShifts = new List<TimeSheetShiftDto> {
                    new TimeSheetShiftDto {
                        ShiftIds = clockingDto.ShiftId.ToString(),
                        RepeatDaysOfWeek = clockingDto.TimeSheet?.TimeSheetShifts.FirstOrDefault()?.RepeatDaysOfWeek
                    }
                }
            };

            var resultCreate = await _createTimeSheetClockingDomainService.CreateWhenReplaceEmployeeAsync(timeSheetDto);
            if (resultCreate.IsValid) return true;

            NotifyValidationErrors(typeof(TimeSheet), resultCreate.ValidationErrors);
            return false;
        }

        private void NotifyUpdateClockingInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(Clocking).Name, Message.clocking_notExist));
        }
        async Task<bool> IsHavePermissionUpdateClockingOnBranch(int branchId, string permissionName, bool isAdmin)
        {
            if (isAdmin)
                return true;
            var permission = await _kiotVietServiceClient.GetPermissionByBranchId(_authService.Context.User.Id, branchId);
            if (!permission.Data.ContainsKey(permissionName) || !permission.Data[permissionName])
            {
                return false;
            }
            return true;
        }
    }
}
