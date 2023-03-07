using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
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

namespace KiotVietTimeSheet.Application.Commands.UpdateClockingMultipleBranchShiftAndDateTime
{
    public class UpdateClockingMultipleBranchShiftAndDateTimeCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateClockingMultipleBranchShiftAndDateTimeCommand, ClockingDto>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IMapper _mapper;
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IAuthService _authService;

        public UpdateClockingMultipleBranchShiftAndDateTimeCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            IAuthService authService,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IKiotVietServiceClient kiotVietServiceClient
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _mapper = mapper;
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _kiotVietServiceClient = kiotVietServiceClient;
            _authService = authService;
        }

        public async Task<ClockingDto> Handle(UpdateClockingMultipleBranchShiftAndDateTimeCommand request, CancellationToken cancellationToken)
        {
            var clockingId = request.ClockingId;
            var shiftTargetId = request.ShiftTargetId;
            var workingDay = request.WorkingDay;
            var branchId = request.BranchId;
            var isHavePermission = await IsHavePermissionBranchInUpdateClockingMultiple(branchId, TimeSheetPermission.Clocking_Update, _authService.Context.User.IsAdmin);
            if (!isHavePermission)
            {
                string branchName = string.Empty;
                var branch = await _kiotVietServiceClient.GetBranchById(branchId);
                if (branch != null)
                    branchName = branch.Name;
                string message = string.Format(Message.clocking_updateDragDropError, branchName);
                NotifyValidationErrors(typeof(Clocking), new List<string> { message });
                return null;
            }
            var oldClocking = await _clockingWriteOnlyRepository.FindBySpecificationAsync(new FindClockingByClockingIdSpec(clockingId)
                .Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void)));
            var shiftTarget = await _shiftReadOnlyRepository.FindByIdAsync(shiftTargetId);

            if (oldClocking == null)
            {
                NotifyUpdateClockingMultipleInDbIsNotExists();
                return null;
            }

            var newClocking = oldClocking.CreateCopy();
            if (shiftTarget != null)
            {
                newClocking.UpdateClockingShiftAndDateTime(shiftTargetId, branchId, workingDay, shiftTarget);
            }

            var validateResult = await (new UpdateClockingShiftAndDateTimeValidator(_clockingReadOnlyRepository, _shiftReadOnlyRepository, oldClocking.ShiftId, shiftTargetId, newClocking.StartTime, newClocking.EndTime)).ValidateAsync(newClocking);
            if (!validateResult.IsValid)
            {
                NotifyValidationErrors(typeof(Clocking), validateResult.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }

            _clockingWriteOnlyRepository.Update(newClocking);
            var listClockingForIntegrationEvent = new List<Tuple<Clocking, Clocking>> { new Tuple<Clocking, Clocking>(oldClocking, newClocking) };
            await _timeSheetIntegrationEventService.AddEventAsync(new UpdateMultipleClockingIntegrationEvent(listClockingForIntegrationEvent));
            await _clockingWriteOnlyRepository.UnitOfWork.CommitAsync();

            var result = _mapper.Map<ClockingDto>(newClocking);
            return result;
        }

        private void NotifyUpdateClockingMultipleInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(Clocking).Name, Message.clocking_notExist));
        }
        async Task<bool> IsHavePermissionBranchInUpdateClockingMultiple(int branchId, string permissionName, bool isAdmin)
        {
            if (isAdmin) return true;

            var permission = await _kiotVietServiceClient.GetPermissionByBranchId(_authService.Context.User.Id, branchId);

            return permission.Data.ContainsKey(permissionName) && permission.Data[permissionName];
        }
    }
}
