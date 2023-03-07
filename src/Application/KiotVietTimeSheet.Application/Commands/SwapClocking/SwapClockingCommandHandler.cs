using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.Validators.ClockingValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Commands.SwapClocking
{
    public class SwapClockingCommandHandler : BaseCommandHandler,
        IRequestHandler<SwapClockingCommand, SwapClockingResultDto>
    {
        private readonly IMapper _mapper;
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IAuthService _authService;

        public SwapClockingCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            IAuthService authService,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IKiotVietServiceClient kiotVietServiceClient
        )
            : base(eventDispatcher)
        {
            _mapper = mapper;
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _kiotVietServiceClient = kiotVietServiceClient;
            _authService = authService;
        }

        public async Task<SwapClockingResultDto> Handle(SwapClockingCommand request, CancellationToken cancellationToken)
        {
            var sourceId = request.SourceId;
            var targetId = request.TargetId;
            var sourceEmployeeId = request.SourceEmployeeId;
            var targetEmployeeId = request.TargetEmployeeId;
            // Parameters validation
            var errors = new List<string>();

            if (sourceEmployeeId == 0 || targetEmployeeId == 0)
            {
                errors.Add(Message.swapShift_haveNotSelectedEmployee);

                NotifyValidationErrors(typeof(Clocking), errors);

                return null;
            }

            if (sourceId == 0 || targetId == 0)
            {
                errors.Add(Message.swapShift_haveNotSeletedShift);

                NotifyValidationErrors(typeof(Clocking), errors);

                return null;
            }
            // End

            // Get clocking data

            var source = await _clockingWriteOnlyRepository.FindBySpecificationAsync(new FindClockingByClockingIdSpec(sourceId));
            var sourceEmployee = await _employeeReadOnlyRepository.FindByIdAsync(source.EmployeeId, false, true);
            var target = await _clockingWriteOnlyRepository.FindBySpecificationAsync(new FindClockingByClockingIdSpec(targetId));
            var targetEmployee = await _employeeReadOnlyRepository.FindByIdAsync(target.EmployeeId, false, true);
            var oldTarget = target.CreateCopy();
            var oldSource = source.CreateCopy();
            // End
            // Check permission
            var isHavePermission = await IsHavePermissionOnBranch(target.BranchId, TimeSheetPermission.Clocking_Update, _authService.Context.User.IsAdmin);
            if (!isHavePermission)
            {
                string branchName = string.Empty;
                var branch = await _kiotVietServiceClient.GetBranchById(target.BranchId);
                if (branch != null)
                    branchName = branch.Name;
                string message = string.Format(Message.clocking_updateError, branchName);
                NotifyValidationErrors(typeof(Clocking), new List<string> { message });
                return null;
            }


            target.SwapShiftWithoutAddDomainEvent(
                source.ShiftId,
                source.StartTime,
                source.EndTime
                );

            source.SwapShift(
                oldTarget,
                oldSource
            );
            var validator = await (new SwapClockingValidator(_clockingReadOnlyRepository, _shiftReadOnlyRepository, target, sourceEmployee, targetEmployee)).ValidateAsync(source);
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(Clocking), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }
            _clockingWriteOnlyRepository.BatchUpdate(new List<Clocking> { source, target });

            await _timeSheetIntegrationEventService.AddEventAsync(new SwappedClockingIntegrationEvent(new SwappedClockingEvent(source, target)));

            await _clockingWriteOnlyRepository.UnitOfWork.CommitAsync(false);
            var swapResult = new SwapClockingResultDto(_mapper.Map<ClockingDto>(source), _mapper.Map<ClockingDto>(target));
            return swapResult;
        }

        async Task<bool> IsHavePermissionOnBranch(int branchId, string permissionName, bool isAdmin)
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
