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
using KiotVietTimeSheet.Application.Parameters.Interfaces;
using KiotVietTimeSheet.Application.Queries.GetSetting;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Utilities;
using MediatR;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.Commands.UpdateConfirmClocking
{
    public class UpdateConfirmClockingCommandHandle :
        IRequestHandler<UpdateConfirmClockingCommand, List<ConfirmClockingDto>>
    {
        private readonly IConfirmClockingWriteOnlyRepository _confirmClockingWriteOnlyRepository;
        private readonly IConfirmClockingHistoryWriteOnlyRepository _confirmClockingHistoryWriteOnlyRepository;
        private readonly IAuthService AuthService;
        private readonly IMapper _mapper;
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;
        private readonly IClockingHistoryWriteOnlyRepository _clockingHistoryWriteOnlyRepository;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly IMediator _mediator;
        private readonly ICalculateTimeClockingDomainService _calculateTimeClockingDomainService;
        private readonly IAutoTimeKeepingDomainService _autoTimeKeepingDomainService;
        private readonly List<ConfirmClockingHistory> _lsConfirmClockingHistoryInsert;
        private readonly List<Employee> _lsEmployeeUpdate;
        private List<Clocking> _lsClockingIn;
        private List<Clocking> _lsClockingOut;
        private List<ClockingHistory> _lsClockingHistory;
        private List<Clocking> _lsClockingNeedUpdate;
        private readonly IConfirmClockingDomainService _confirmClockingDomainService;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;

        private readonly IUpdateConfirmClockingParamBuilder _updateConfirmClockingParamBuilder;

        public UpdateConfirmClockingCommandHandle(
            IMapper mapper,
            IAuthService authService,
            IMediator mediator,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IUpdateConfirmClockingParamBuilder updateConfirmClockingParamBuilder
            )
        {
            this._mapper = mapper;
            _updateConfirmClockingParamBuilder = updateConfirmClockingParamBuilder;
            this._confirmClockingWriteOnlyRepository = _updateConfirmClockingParamBuilder.GetConfirmClockingWriteOnlyRepository();
            this._confirmClockingHistoryWriteOnlyRepository = _updateConfirmClockingParamBuilder.GetConfirmClockingHistoryWriteOnlyRepository();
            this.AuthService = authService;
            _employeeWriteOnlyRepository = _updateConfirmClockingParamBuilder.GetEmployeeWriteOnlyRepository();
            _clockingHistoryWriteOnlyRepository = _updateConfirmClockingParamBuilder.GetClockingHistoryWriteOnlyRepository();
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _mediator = mediator;
            _calculateTimeClockingDomainService = _updateConfirmClockingParamBuilder.GetCalculateTimeClockingDomainService();
            _autoTimeKeepingDomainService = _updateConfirmClockingParamBuilder.GetAutoTimeKeepingDomainService();
            _lsConfirmClockingHistoryInsert = new List<ConfirmClockingHistory>();
            _lsEmployeeUpdate = new List<Employee>();
            _lsClockingIn = new List<Clocking>();
            _lsClockingOut = new List<Clocking>();
            _lsClockingHistory = new List<ClockingHistory>();
            _lsClockingNeedUpdate = new List<Clocking>();
            _confirmClockingDomainService = _updateConfirmClockingParamBuilder.GetConfirmClockingDomainService();
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;

        }
        public async Task<List<ConfirmClockingDto>> Handle(UpdateConfirmClockingCommand request, CancellationToken cancellationToken)
        {
            var lsConfirmClockingUpdateDto = request.LsConfirmClockingDto;
            var lsConfirmClockingUpdateIds = lsConfirmClockingUpdateDto.Select(x => x.Id).ToList();
            var lsConfirmClockingUpdate = (await _confirmClockingWriteOnlyRepository.GetBySpecificationAsync(new GetConfirmClokingByIdsSpec(lsConfirmClockingUpdateIds))).OrderBy(x => x.CheckTime).ToList();
            foreach (var cf in lsConfirmClockingUpdate)
            {
                var CFUpdate = lsConfirmClockingUpdateDto.FirstOrDefault(x => x.Id == cf.Id);
                _lsConfirmClockingHistoryInsert.Add(CreateConfirmClockingHistory(cf.Id, CFUpdate.Status, cf.Status));
                cf.Status = CFUpdate.Status;

                // Update employee identity key clocking
                if (_confirmClockingDomainService.IsTypeNewDevice(cf.Type) && cf.Status == (byte)ConfirmClockingStatus.Confirm)
                {
                    await UpdateEmployeeIdentityKeyclocking(cf);
                }

                if (CFUpdate.Status == (byte)ConfirmClockingStatus.Confirm)
                {
                    await ConfirmClocking(cf, cancellationToken);
                }
            }
            _employeeWriteOnlyRepository.BatchUpdate(_lsEmployeeUpdate);
            _confirmClockingWriteOnlyRepository.BatchUpdate(lsConfirmClockingUpdate);
            _confirmClockingHistoryWriteOnlyRepository.BatchAdd(_lsConfirmClockingHistoryInsert);

            var clockings = BuildClockings(OrderGroupClockings(_lsClockingIn, true), OrderGroupClockings(_lsClockingOut, false));
            _lsClockingNeedUpdate.AddRange(clockings);

            _clockingWriteOnlyRepository.BatchUpdate(_lsClockingNeedUpdate);
            _clockingHistoryWriteOnlyRepository.BatchAdd(_lsClockingHistory);

            await _confirmClockingWriteOnlyRepository.UnitOfWork.CommitAsync();
            var result = _mapper.Map<List<ConfirmClockingDto>>(lsConfirmClockingUpdate);

            var internalEvent = new CreatedConfirmClockingIntegrationEvent();
            await _timeSheetIntegrationEventService.PublishEventsToEventBusAsync(internalEvent);

            return result;
        }

        private async Task UpdateEmployeeIdentityKeyclocking(ConfirmClocking cf)
        {
            var employee = (await _employeeWriteOnlyRepository.GetBySpecificationAsync(new FindEmployeeByIdSpec(cf.EmployeeId))).First();
            var epployeeInlist = _lsEmployeeUpdate.FirstOrDefault(em => em.Id == employee.Id);
            if (epployeeInlist == null)
            {
                employee.UpdateIdentityKeyClocking(cf.IdentityKeyClocking);
                _lsEmployeeUpdate.Add(employee);
            }
            else
                epployeeInlist.UpdateIdentityKeyClocking(cf.IdentityKeyClocking);
        }

        private async Task<bool> HandleClockingMultiple(ConfirmClocking cf, Clocking clocking, bool isCheckIn)
        {
            if (_lsClockingNeedUpdate.Any(x => x.Id == clocking.Id)) return false;

            var oldClocking = (await _clockingWriteOnlyRepository.GetBySpecificationAsync(new FindClockingByIdsSpec(new List<long> { clocking.Id })))?.FirstOrDefault();
            if (oldClocking == null) return false;

            // Bỏ qua những chấm công ra có giờ nhỏ hơn chấm công ra trước đó.
            if (!isCheckIn && oldClocking.CheckOutDate != null && oldClocking.CheckOutDate > clocking.CheckOutDate)
                return true;

            if (isCheckIn && oldClocking.CheckInDate != null)
                return true;

            var lsClockingRelate = _lsClockingIn.Where(x => x.EmployeeId == cf.EmployeeId).ToList();
            lsClockingRelate.AddRange(_lsClockingOut.Where(x => x.EmployeeId == cf.EmployeeId).ToList());
            var lstClokingMultiple = await _autoTimeKeepingDomainService.GetClokingMultiple(cf.CheckTime, cf.EmployeeId, oldClocking, isCheckIn, lsClockingRelate);
            if (lstClokingMultiple?.Any() == true)
            {
                // Trường hợp cả 2 ca chấm công liên tiếp đều cần xác nhận chấm công
                var lsClockingIdRemove = lstClokingMultiple.Select(x => x.Id).ToList();
                _lsClockingIn = _lsClockingIn.Where(x => !lsClockingIdRemove.Contains(x.Id)).ToList();
                _lsClockingOut = _lsClockingOut.Where(x => !lsClockingIdRemove.Contains(x.Id)).ToList();
                _lsClockingNeedUpdate = _lsClockingNeedUpdate.Where(x => !lsClockingIdRemove.Contains(x.Id)).ToList();
                _lsClockingHistory = _lsClockingHistory.Where(x => !lsClockingIdRemove.Contains(x.ClockingId)).ToList();

                // Thêm các ctlv lấy được từ chấm công 2 ca liên tiếp vào dữ liệu cần lưu trữ
                _lsClockingNeedUpdate.AddRange(lstClokingMultiple);

                return true;
            }

            return false;
        }

        private async Task ConfirmClocking(ConfirmClocking cf, CancellationToken cancellationToken)
        {
            // Update clocking
            var extra = JsonConvert.DeserializeObject<ConfirmClockingExtra>(cf.Extra);
            Clocking clocking = extra.Clocking;

            // Xử lý chấm công 2 ca liên tiếp
            var hasClockingMultiple = await HandleClockingMultiple(cf, clocking, extra.IsCheckIn);
            if (hasClockingMultiple) return;

            // Xử lý chấm công từng ctlv đơn lẻ
            var clockingHistory = extra.ClockingHistory;
            var timeIsLate = 1;
            var overTimeBeforeShiftWork = 1;
            var timeIsLeaveWorkEarly = 1;
            var overTimeAfterShiftWork = 1;
            byte? absenceType = null;
            DateTime? checkTime = null;
            if (clockingHistory != null)
            {
                checkTime = clockingHistory.CheckTime;
                timeIsLate = await _calculateTimeClockingDomainService.GetTimeLate(AuthService.Context.TenantId, clocking.StartTime, clocking.CheckInDate);
                overTimeBeforeShiftWork = await _calculateTimeClockingDomainService.GetOverTimeBeforeShiftWork(AuthService.Context.TenantId, clocking.StartTime, clocking.CheckInDate);
                timeIsLeaveWorkEarly = await _calculateTimeClockingDomainService.GetTimeEarly(AuthService.Context.TenantId, clocking.EndTime, clocking.CheckOutDate);
                overTimeAfterShiftWork = await _calculateTimeClockingDomainService.GetOverTimeAfterShiftWork(AuthService.Context.TenantId, clocking.EndTime, clocking.CheckOutDate);
                absenceType = clockingHistory.AbsenceType;
            }

            var settingsObjectDto = await _mediator.Send(new GetSettingQuery(this.AuthService.Context.TenantId), cancellationToken);

            var newClockingHistoryItem = ClockingHistory.GenerateClockingHistory(
                clocking.Id,
                clocking.BranchId,
                clocking.TenantId,
                clocking.CheckInDate,
                clocking.CheckOutDate,
                clocking.StartTime,
                clocking.EndTime,
                (byte)TimeKeepingTypes.Gps,
                settingsObjectDto,
                clocking.ClockingStatus,
                timeIsLate,
                overTimeBeforeShiftWork,
                timeIsLeaveWorkEarly,
                overTimeAfterShiftWork,
                clocking.AbsenceType,
                absenceType,
                false,
                clocking.ShiftId,
                clocking.EmployeeId,
                clocking.ClockingHistories?.OrderByDescending(ch => ch.CreatedDate).FirstOrDefault(),
                checkTime,
                null,
                extra.CheckInDateType,
                extra.CheckOutDateType
            );

            ClassifyClocking(_lsClockingIn, _lsClockingOut, clocking, extra.IsCheckIn);
            _lsClockingHistory.Add(newClockingHistoryItem);
        }

        private ConfirmClockingHistory CreateConfirmClockingHistory(long confirmClockingId, byte statusNew, byte statusOld)
        {
            var confirmClockingHistory = new ConfirmClockingHistory();
            confirmClockingHistory.ConfirmClockingId = confirmClockingId;
            confirmClockingHistory.StatusNew = statusNew;
            confirmClockingHistory.StatusOld = statusOld;
            confirmClockingHistory.ConfirmBy = AuthService.Context.User.Id;
            return confirmClockingHistory;

        }

        private void ClassifyClocking(List<Clocking> lsClockingIn, List<Clocking> lsClockingOut, Clocking clocking, bool isCheckIn)
        {
            if (isCheckIn)
                lsClockingIn.Add(clocking);
            else
                lsClockingOut.Add(clocking);
        }

        private List<Clocking> OrderGroupClockings(List<Clocking> clockings, bool isCheckIn)
        {
            clockings = isCheckIn ? clockings.OrderBy(c => c.CheckInDate).ToList() : clockings.OrderByDescending(c => c.CheckOutDate).ToList();
            return (clockings.GroupBy(c => (c.Id)).Select(g => g.First())).ToList();
        }

        private List<Clocking> BuildClockings(List<Clocking> clockingsIn, List<Clocking> clockingsOut)
        {
            var clockingMerged = new List<Clocking>();
            foreach (var clocking in clockingsIn)
            {
                var clockingOut = clockingsOut.Find(c => c.Id == clocking.Id);
                if (clockingOut != null)
                {
                    clockingOut.CheckInDate = clocking.CheckInDate;
                    clockingOut.UpdateClockingStatus((byte)ClockingStatuses.CheckedOut);
                    clockingMerged.Add(clockingOut);
                    clockingsOut.Remove(clockingOut);
                }
                else
                {
                    clockingMerged.Add(clocking);
                }
            }
            clockingMerged.AddRange(clockingsOut);

            return clockingMerged;
        }

    }
}
