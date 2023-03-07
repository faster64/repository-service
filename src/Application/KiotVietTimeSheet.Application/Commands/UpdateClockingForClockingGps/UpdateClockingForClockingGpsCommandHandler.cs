using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.ConfirmClockingEvents;
using KiotVietTimeSheet.Application.Queries.GetSetting;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.ClockingGpsValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Validators;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Utilities;
using MediatR;
using ServiceStack;
using static KiotVietTimeSheet.Domain.Utilities.Utilities;

namespace KiotVietTimeSheet.Application.Commands.UpdateClockingForClockingGps
{
    public class UpdateClockingForClockingGpsCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateClockingForClockingGpsCommand, ClockingDto>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IMapper _mapper;
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly IClockingPenalizeWriteOnlyRepository _clockingPenalizeWriteOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly IClockingHistoryWriteOnlyRepository _clockingHistoryWriteOnlyRepository;
        private readonly IAuthService _authService;
        private readonly IConfirmClockingWriteOnlyRepository _confirmClockingWriteOnlyRepository;
        private readonly IGpsInfoReadOnlyRepository _gpsInfoReadOnlyRepository;
        private readonly IMediator _mediator;
        private GpsInfo _gpsInfo;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private Employee _employee;
        private readonly ICalculateTimeClockingDomainService _calculateTimeClockingDomainService;
        private readonly IAutoTimeKeepingDomainService _autoTimeKeepingDomainService;
        private readonly IConfirmClockingDomainService _confirmClockingDomainService;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;

        public UpdateClockingForClockingGpsCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            IClockingPenalizeWriteOnlyRepository clockingPenalizeWriteOnlyRepository,
            IAuthService authService,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            IClockingHistoryWriteOnlyRepository clockingHistoryWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IConfirmClockingWriteOnlyRepository confirmClockingWriteOnlyRepository,
            IGpsInfoReadOnlyRepository gpsInfoReadOnlyRepository,
            IMediator mediator,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            ICalculateTimeClockingDomainService calculateTimeClockingDomainService,
            IAutoTimeKeepingDomainService autoTimeKeepingDomainService,
            IConfirmClockingDomainService confirmClockingDomainService
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _mapper = mapper;
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _clockingHistoryWriteOnlyRepository = clockingHistoryWriteOnlyRepository;
            _authService = authService;
            _mediator = mediator;
            _confirmClockingWriteOnlyRepository = confirmClockingWriteOnlyRepository;
            _gpsInfoReadOnlyRepository = gpsInfoReadOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _calculateTimeClockingDomainService = calculateTimeClockingDomainService;
            _autoTimeKeepingDomainService = autoTimeKeepingDomainService;
            _confirmClockingDomainService = confirmClockingDomainService;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _clockingPenalizeWriteOnlyRepository = clockingPenalizeWriteOnlyRepository;
        }

        public async Task<ClockingDto> Handle(UpdateClockingForClockingGpsCommand request, CancellationToken cancellationToken)
        {
            if (!(await Validate(request))) return null;

            var clockingDto = request.ClockingDto;
            var clockingHistoryDto = request.ClockingHistoryDto;
            var extra = new ConfirmClockingExtra();

            var oldClocking = await _clockingWriteOnlyRepository.FindBySpecificationWithIncludesAsync(new FindClockingByClockingIdSpec(clockingDto.Id)
                .Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void)), null);

            if (oldClocking != null)
            {
                var clockingHistories = await _clockingHistoryWriteOnlyRepository.GetBySpecificationAsync(new FindClockingHistoryByClockingIdSpec(clockingDto.Id), null);
                var clockingPenalizes = await _clockingPenalizeWriteOnlyRepository.GetBySpecificationAsync(new FindClockingPenalizeByClockingIdSpec(clockingDto.Id), null);
                oldClocking.ClockingHistories = clockingHistories;
                oldClocking.ClockingPenalizes = clockingPenalizes;
            }

            var shiftTarget = await _shiftReadOnlyRepository.GetShiftByIdForClockingGps(clockingDto.ShiftId, false);

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

            var validateResult = await (new UpdateClockingGpsValidator(_clockingReadOnlyRepository, _shiftReadOnlyRepository, oldClocking.ShiftId, clockingDto.ShiftId, newClockingItem.StartTime, newClockingItem.EndTime)).ValidateAsync(newClockingItem);
            if (!validateResult.IsValid)
            {
                NotifyValidationErrors(typeof(Clocking), validateResult.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }

            var settingsObjectDto = await _mediator.Send(new GetSettingQuery(_authService.Context.TenantId), cancellationToken);
            ClockingHistory newClockingHistoryItem = null;

            if (shiftTarget != null)
            {
                clockingHistoryDto.TimeIsLate = await _calculateTimeClockingDomainService.GetTimeLate(_authService.Context.TenantId, clockingDto.StartTime, clockingHistoryDto.CheckedInDate);
                clockingHistoryDto.TimeIsLeaveWorkEarly = await _calculateTimeClockingDomainService.GetTimeEarly(_authService.Context.TenantId, clockingDto.EndTime, clockingHistoryDto.CheckedOutDate);
                clockingHistoryDto.OverTimeBeforeShiftWork = await _calculateTimeClockingDomainService.GetOverTimeBeforeShiftWork(_authService.Context.TenantId, clockingDto.StartTime, clockingHistoryDto.CheckedInDate);
                clockingHistoryDto.OverTimeAfterShiftWork = await _calculateTimeClockingDomainService.GetOverTimeAfterShiftWork(_authService.Context.TenantId, clockingDto.EndTime, clockingHistoryDto.CheckedOutDate);

                newClockingHistoryItem = ClockingHistory.GenerateClockingHistory(
                    newClockingItem.Id,
                    newClockingItem.BranchId,
                    newClockingItem.TenantId,
                    clockingHistoryDto.CheckedInDate,
                    clockingHistoryDto.CheckedOutDate,
                    newClockingItem.StartTime,
                    newClockingItem.EndTime,
                    (byte)TimeKeepingTypes.Gps,
                    settingsObjectDto,
                    newClockingItem.ClockingStatus,
                    clockingHistoryDto.TimeIsLate,
                    clockingHistoryDto.OverTimeBeforeShiftWork,
                    clockingHistoryDto.TimeIsLeaveWorkEarly,
                    clockingHistoryDto.OverTimeAfterShiftWork,
                    oldClocking.AbsenceType,
                    clockingHistoryDto.AbsenceType,
                    false,
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

                    AddClockingHistoryToExtra(request, newClockingHistoryItem, clockingHistoryDto, extra);
                }
            }

            //Check identityKeyClocking
            extra.Clocking = newClockingItem;
            var addConfirm = await AddConfirmClocking(request, extra);
            if (!addConfirm)
            {
                return null;
            }

            // Xử lý 2 ca liền nhau và lưu data
            await HandleClockingMultiAndStore(request, oldClocking, newClockingItem, newClockingHistoryItem);

            var result = _mapper.Map<ClockingDto>(newClockingItem);
            return result;
        }

        /// <summary>
        /// Xử lý 2 ca liền nhau và lưu data
        /// </summary>
        private async Task HandleClockingMultiAndStore(UpdateClockingForClockingGpsCommand request, Clocking oldClocking, Clocking newClockingItem, ClockingHistory newClockingHistoryItem)
        {
            var isCheckIn = request.ClockingDto.CheckInDate != null && request.ClockingDto.CheckOutDate == null;
            var lstClokingMultiple = await _autoTimeKeepingDomainService.GetClokingMultiple(request.ClockingHistoryDto.CheckTime ?? DateTime.Now, request.ClockingDto.EmployeeId, oldClocking, isCheckIn);
            if (lstClokingMultiple?.Any() == true)
            {
                _clockingWriteOnlyRepository.BatchUpdate(lstClokingMultiple);
            }
            else
            {
                _clockingWriteOnlyRepository.Update(newClockingItem);

                if (newClockingHistoryItem != null)
                {
                    _clockingHistoryWriteOnlyRepository.Add(newClockingHistoryItem);
                }
            }

            await _clockingWriteOnlyRepository.UnitOfWork.CommitAsync(false);
        }

        private void AddClockingHistoryToExtra(UpdateClockingForClockingGpsCommand request, ClockingHistory newClockingHistoryItem, ClockingHistoryDto clockingHistoryDto, ConfirmClockingExtra extra)
        {
            if ((request.AcceptWrongGps && IsWrongGps(request.GeoCoordinate)) || IsNewDevice(request.IdentityKeyClocking))
            {
                extra.ClockingHistory = newClockingHistoryItem;
                extra.CheckInDateType = clockingHistoryDto.CheckInDateType;
                extra.CheckOutDateType = clockingHistoryDto.CheckOutDateType;
            }
        }

        private async Task<bool> Validate(UpdateClockingForClockingGpsCommand request)
        {
            _gpsInfo = await _gpsInfoReadOnlyRepository.GetGpsInforByBranchId(_authService.Context.TenantId, request.ClockingDto.BranchId);
            if (_gpsInfo == null)
            {
                NotifyValidationErrors(typeof(GpsInfo), new List<string> { "Không tìm thấy thiết lập chấm công GPS." });
                return false;
            }

            _employee = await _employeeReadOnlyRepository.GetByIdWithoutLimit(request.ClockingDto.EmployeeId);
            if (_employee == null)
            {
                NotifyValidationErrors(typeof(GpsInfo), new List<string> { "Không tìm thấy nhân viên." });
                return false;
            }

            if (!(await CheckTimeForClocking(request)))
            {
                return false;
            }

            return true;
        }

        private async Task<bool> AddConfirmClocking(UpdateClockingForClockingGpsCommand req, ConfirmClockingExtra extra)
        {
            var isWrongGps = IsWrongGps(req.GeoCoordinate);
            var isNewDevice = IsNewDevice(req.IdentityKeyClocking);
            if ((req.AcceptWrongGps && isWrongGps) || isNewDevice)
            {
                extra.IsCheckIn = (req.ClockingDto.CheckInDate != null && req.ClockingDto.CheckOutDate == null);
                extra.Clocking.ClearDomainEvents();
                extra.Clocking.ClockingHistories = null;
                var type = (byte)_confirmClockingDomainService.GetConfirmClockingType(isWrongGps, isNewDevice, req.GeoCoordinate);
                var confirmClocking = new ConfirmClocking
                {
                    TenantId = _authService.Context.TenantId,
                    GpsInfoId = _gpsInfo.Id,
                    EmployeeId = req.ClockingDto.EmployeeId,
                    CheckTime = DateTime.Now,
                    Type = type,
                    Status = (byte)ConfirmClockingStatus.Waiting,
                    IdentityKeyClocking = req.IdentityKeyClocking,
                    Extra = JSON.stringify(extra),
                    CreatedBy = 0,
                    CreatedDate = DateTime.Now,
                };

                _confirmClockingWriteOnlyRepository.Add(confirmClocking);
                await _confirmClockingWriteOnlyRepository.UnitOfWork.CommitAsync(false);

                var internalEvent = new CreatedConfirmClockingIntegrationEvent();
                await _timeSheetIntegrationEventService.PublishEventsToEventBusAsync(internalEvent);

                return false;
            }
            return true;
        }

        private async Task<bool> CheckTimeForClocking(UpdateClockingForClockingGpsCommand req)
        {
            // gắn thời gian hiện tại cho chấm công.
            var isCheckIn = (req.ClockingDto.CheckInDate != null && req.ClockingDto.CheckOutDate == null);
            var now = DateTime.Now;
            if (isCheckIn)
            {
                req.ClockingDto.CheckInDate = now;
                req.ClockingHistoryDto.CheckedInDate = now;
                req.ClockingHistoryDto.CheckTime = now;
            }
            else
            {
                req.ClockingDto.CheckOutDate = now;
                req.ClockingHistoryDto.CheckedOutDate = now;
                req.ClockingHistoryDto.CheckTime = now;
            }

            var start = req.ClockingDto.StartTime.AddHours(-GetHoursFromMinute(req.ClockingDto.Shift?.CheckInBefore, req.ClockingDto.Shift?.From));
            var end = req.ClockingDto.EndTime.AddHours(GetHoursFromMinute(req.ClockingDto.Shift?.To, req.ClockingDto.Shift?.CheckOutAfter));

            if (!req.AcceptWrongGps && IsWrongGps(req.GeoCoordinate))
            {
                await _eventDispatcher.FireEvent(new DomainNotification(typeof(ConfirmClocking).Name, new ErrorResult { Code = "wrong_gps", Message = "Hệ thống không xác nhận bạn đang trong khu vực làm việc." }));
                return false;
            }

            if (req.ClockingHistoryDto.CheckTime < start)
            {
                await _eventDispatcher.FireEvent(new DomainNotification(typeof(Clocking).Name, "Chưa đến thời gian được phép chấm công. Vui lòng chấm công trong thời gian cho phép."));
                return false;
            }

            if (req.ClockingHistoryDto.CheckTime > end)
            {
                await _eventDispatcher.FireEvent(new DomainNotification(typeof(Clocking).Name, "Đã hết thời gian chấm công. Vui lòng chấm công trong thời gian cho phép."));
                return false;
            }

            return true;
        }

        private bool IsWrongGps(string geoCoordinate)
        {
            if (string.IsNullOrEmpty(geoCoordinate)) return true;

            var lat1 = double.Parse(_gpsInfo.Coordinate.Split(',')[0]);
            var long1 = double.Parse(_gpsInfo.Coordinate.Split(',')[1]);

            var lat2 = double.Parse(geoCoordinate.Split(',')[0]);
            var long2 = double.Parse(geoCoordinate.Split(',')[1]);

            var distance = Globals.GetDistance(lat1, long1, lat2, long2);
            var r = distance > _gpsInfo.RadiusLimit;

            return r;
        }

        private bool IsNewDevice(string identityKeyClocking)
        {
            var r = _employee.IdentityKeyClocking != identityKeyClocking;
            return r;
        }

        private void NotifyUpdateClockingInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(Clocking).Name, Resources.Message.clocking_notExist));
        }
    }
}
