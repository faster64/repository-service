using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.AutoTimeKeepingEvents;
using KiotVietTimeSheet.Application.Queries.GetSetting;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications;
using KiotVietTimeSheet.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using static KiotVietTimeSheet.Domain.Utilities.Utilities;

namespace KiotVietTimeSheet.Application.DomainService.Impls
{
    public class AutoTimeKeepingDomainService : IAutoTimeKeepingDomainService
    {
        private readonly IAuthService _authService;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly IFingerPrintReadOnlyRepository _fingerPrintReadOnlyRepository;
        private readonly IClockingHistoryWriteOnlyRepository _clockingHistoryWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IFingerMachineWriteOnlyRepository _fingerMachineWriteOnlyRepository;
        private readonly IPaySheetOutOfDateDomainService _paySheetOutOfDateDomainService;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly List<Clocking> _clockingsNeedUpdate = new List<Clocking>();
        private readonly List<ClockingHistory> _clockingsHistoryNeedAdd = new List<ClockingHistory>();
        private readonly ILogger<AutoTimeKeepingDomainService> _logger;
        private readonly IMediator _mediator;

        public AutoTimeKeepingDomainService(
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            IFingerPrintReadOnlyRepository fingerPrintReadOnlyRepository,
            IClockingHistoryWriteOnlyRepository clockingHistoryWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IAuthService authService,
            IFingerMachineWriteOnlyRepository fingerMachineWriteOnlyRepository,
            IPaySheetOutOfDateDomainService paySheetOutOfDateDomainService,
            ILogger<AutoTimeKeepingDomainService> logger,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            IMediator mediator
            )
        {
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _fingerPrintReadOnlyRepository = fingerPrintReadOnlyRepository;
            _clockingHistoryWriteOnlyRepository = clockingHistoryWriteOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _authService = authService;
            _fingerMachineWriteOnlyRepository = fingerMachineWriteOnlyRepository;
            _paySheetOutOfDateDomainService = paySheetOutOfDateDomainService;
            _logger = logger;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _mediator = mediator;
        }

        public async Task<List<Clocking>> GetClokingMultiple(DateTime checkDateTime, long employeeId, Clocking clockingTarget, bool isCheckIn, List<Clocking> lsClockingRelate = null)
        {
            var settingObjectDto = await _mediator.Send(new GetSettingQuery(_authService.Context.TenantId));
            var isAutoTimekeepingCondition = IsAutoTimeKeepingMultiple(settingObjectDto);

            // Có cho phép chấm công 2 ca liên tiếp không?
            if (!isAutoTimekeepingCondition) return null;

            // Lấy các ctlv trong thời gian cho phép
            var startTimeLimit = checkDateTime.AddHours(-Constant.MaximumCheckInCheckOutHours);
            var endTimeLimit = checkDateTime.AddHours(Constant.MaximumCheckInCheckOutHours);
            var clockings = await _clockingWriteOnlyRepository
                    .GetBySpecificationAsync(
                       spec: new FindClockingByBranchIdSpec(_authService.Context.BranchId)
                                .And(new FindClockingByEmployeeIdSpec(employeeId))
                                .And(new FindClockingByEndTimeGreaterThanOrEqualSpec(startTimeLimit))
                                .And(new FindClockingByStartTimeLessThanOrEqualSpec(endTimeLimit))
                                .Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void))
                    );

            if (lsClockingRelate?.Any() == true)
            {
                clockings = clockings.Where(x => lsClockingRelate.Select(y => y.Id).All(z => z != x.Id)).ToList();
                clockings.AddRange(lsClockingRelate);
            }

            var lstClockingNeedUpdate = new List<Clocking>();
            var clockingTargetCopy = clockingTarget.CreateCopy();

            // Lấy danh sách các ctlv hợp lệ (không bao gồm clockingTarget)
            var lstBeforeClockingTarget = FindClockingForTimeKeepingMultiple(checkDateTime, clockingTargetCopy, clockings, settingObjectDto);
            if (lstBeforeClockingTarget?.Any() != true) return null;

            // Cập nhập trang thái chấm công
            if (isCheckIn)
            {
                clockingTarget.UpdateClockingCheckedInDate(checkDateTime, settingObjectDto);
                clockingTarget.UpdateClockingStatus((byte)ClockingStatuses.CheckedIn);
            }
            else
            {
                clockingTarget.UpdateClockingCheckedInDate(clockingTarget.StartTime, settingObjectDto);
                clockingTarget.UpdateClockingCheckedOutDate(checkDateTime, settingObjectDto);
                clockingTarget.UpdateClockingStatus((byte)ClockingStatuses.CheckedOut);
            }

            // Ghi nhận lịch sử chấm công
            var clockingHistoryTarget = CreateClockingHistory(clockingTarget, checkDateTime, null, (byte)TimeKeepingTypes.Gps, settingObjectDto);
            if (clockingHistoryTarget != null)
            {
                clockingTarget.ClockingHistories = clockingTarget.ClockingHistories ?? new List<ClockingHistory>();
                clockingTarget.ClockingHistories.Add(clockingHistoryTarget);
            }

            // Cập nhật trạng thái các ctlv trước đó
            lstBeforeClockingTarget.ForEach(c =>
            {
                if (c.CheckInDate == null) c.UpdateClockingCheckedInDate(c.StartTime, settingObjectDto);
                if (c.CheckOutDate == null) c.UpdateClockingCheckedOutDate(c.EndTime, settingObjectDto);
                c.UpdateClockingStatus((byte)ClockingStatuses.CheckedOut);

                // Ghi nhận lịch sử chấm công
                var beforeClockingHistory = CreateClockingHistory(c, checkDateTime, null, (byte)TimeKeepingTypes.Gps, settingObjectDto);
                if (beforeClockingHistory != null)
                {
                    c.ClockingHistories = c.ClockingHistories ?? new List<ClockingHistory>();
                    c.ClockingHistories.Add(beforeClockingHistory);
                }
            });

            lstClockingNeedUpdate.Add(clockingTarget);
            lstClockingNeedUpdate.AddRange(lstBeforeClockingTarget);

            return lstClockingNeedUpdate;
        }

        /// <summary>
        /// Thực hiện chấm công tự động cho danh sách các dữ liệu chấm công nhận được
        /// </summary>
        /// <param name="fingerPrintLogs"></param>
        /// <returns></returns>
        public async Task<List<AutoTimeKeepingResult>> AutoTimeKeepingAsync(List<FingerPrintLogDto> fingerPrintLogs)
        {
            var autoTimeKeepingResults = new List<AutoTimeKeepingResult>();
            var listClockingForUpdatePaySheet = new List<Clocking>();
            var settingObjectDto = await _mediator.Send(new GetSettingQuery(_authService.Context.TenantId));
            var fingerCodes = fingerPrintLogs.Select(fingerItem => fingerItem.FingerCode).Distinct().ToList();
            var employees = await _fingerPrintReadOnlyRepository
                .GetBySpecificationAsync(
                    new FindFingerPrintByBranchIdSpec(_authService.Context.BranchId)
                        .And(new GetAllFingerPrintByFingerCodesSpec(fingerCodes)));
            var listClocking = await GetClockingForTimeKeepingAsync(fingerPrintLogs, employees, settingObjectDto);
            var shifts = await _shiftReadOnlyRepository.GetBySpecificationAsync(
                new FindShiftByBranchIdSpec(_authService.Context.BranchId));

            var isAutoTimekeepingCondition = IsAutoTimeKeepingMultiple(settingObjectDto);

            foreach (var log in fingerPrintLogs.OrderBy(x => x.CheckDateTime).ToList())
            {
                try
                {
                    var clockingTarget = FindClockingForTimeKeeping(log, listClocking, employees, shifts, isAutoTimekeepingCondition);
                    var clockingTargetCopy = clockingTarget.CreateCopy();

                    //Nếu thỏa mãn tự động chấm công 2 ca liền nhau (SRS: #3021)
                    var isConditionAutoCheckInCheckout = BetweenNearClockingAutoCheckInAndCheckout(
                        isAutoTimekeepingCondition, log, clockingTarget, clockingTargetCopy, listClocking,
                        autoTimeKeepingResults, listClockingForUpdatePaySheet,
                        settingObjectDto);
                    if (!isConditionAutoCheckInCheckout) continue;

                    var autoTimeKeepingResult = SingleAutoTimeKeeping(log, clockingTarget, settingObjectDto);
                    autoTimeKeepingResults.Add(autoTimeKeepingResult);

                    AddOrUpdateClockingAutoKeeping(new List<Clocking>() { clockingTarget });

                    if (autoTimeKeepingResult.IsSuccess && !clockingTargetCopy.IsDataNotChanged(clockingTarget) &&
                        !listClockingForUpdatePaySheet.Exists(c => c.Id == clockingTarget?.Id))
                        listClockingForUpdatePaySheet.Add(clockingTarget);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    var autoTimeKeepingResult = AutoTimeKeepingFailed(log, ex.Message);
                    autoTimeKeepingResults.Add(autoTimeKeepingResult);
                }
            }

            _clockingWriteOnlyRepository.BatchUpdate(_clockingsNeedUpdate);
            _clockingHistoryWriteOnlyRepository.BatchAdd(_clockingsHistoryNeedAdd);
            await _paySheetOutOfDateDomainService.WithClockingDataChangeAsync(listClockingForUpdatePaySheet);
            await _timeSheetIntegrationEventService.AddEventAsync(new AutoTimeKeepingIntegrationEvent(autoTimeKeepingResults));
            await _clockingWriteOnlyRepository.UnitOfWork.CommitAsync();

            return autoTimeKeepingResults;
        }

        /// <summary>
        /// Nếu thỏa mãn tự động chấm công 2 ca liền nhau (SRS: #3021)
        /// </summary>
        /// <param name="isAutoTimekeepingCondition"></param>
        /// <param name="log"></param>
        /// <param name="clockingTarget"></param>
        /// <param name="clockingTargetCopy"></param>
        /// <param name="listClocking"></param>
        /// <param name="autoTimeKeepingResults"></param>
        /// <param name="listClockingForUpdatePaySheet"></param>
        /// <param name="settingObjectDto"></param>
        /// <returns></returns>
        private bool BetweenNearClockingAutoCheckInAndCheckout(
            bool isAutoTimekeepingCondition, FingerPrintLogDto log,
            Clocking clockingTarget, Clocking clockingTargetCopy,
            List<Clocking> listClocking, List<AutoTimeKeepingResult> autoTimeKeepingResults,
            List<Clocking> listClockingForUpdatePaySheet,
            SettingObjectDto settingObjectDto)
        {
            if (!isAutoTimekeepingCondition) return true;

            var beforeListClockingTarget = FindClockingForTimeKeepingMultiple(log.CheckDateTime, clockingTarget, listClocking, settingObjectDto);

            if (!beforeListClockingTarget.Any()) return true;

            var beforeListClockingTargetCopy = beforeListClockingTarget.Select(x => x.CreateCopy()).ToList();
            var autoTimeKeepingResultMultiple = AutoTimeKeepingClockings(log, clockingTarget, beforeListClockingTarget, settingObjectDto);

            autoTimeKeepingResults.Add(autoTimeKeepingResultMultiple);

            if (!autoTimeKeepingResultMultiple.IsSuccess) return false;

            //Kiểm tra xem clocking trước có thay đổi (Thêm vào clocking cần thay cập nhập version cho paySheet)
            if (!clockingTargetCopy.IsDataNotChanged(clockingTarget) && !listClockingForUpdatePaySheet.Exists(c => c.Id == clockingTarget?.Id))
            {
                listClockingForUpdatePaySheet.Add(clockingTarget);
            }

            //Kiểm tra xem clocking trước có thay đổi (Thêm vào clocking cần thay cập nhập version cho paySheet)
            var beforeClockingTargetChange = beforeListClockingTarget.Except(beforeListClockingTargetCopy).ToList();
            if (!listClockingForUpdatePaySheet.Any(c => beforeClockingTargetChange.Any(cf => cf.Id == c.Id)))
            {
                listClockingForUpdatePaySheet.AddRange(beforeClockingTargetChange);
            }

            var groupListClocking = new List<Clocking>() { clockingTarget };
            groupListClocking.AddRange(beforeClockingTargetChange);

            AddOrUpdateClockingAutoKeeping(groupListClocking);

            return false;
        }

        /// <summary>
        /// Xử lý ghi nhận chấm công cho ca
        /// </summary>
        /// <param name="fingerPrintLog"></param>
        /// <param name="clockingTarget"></param>
        /// <param name="beforeClockingsTarget"></param>
        /// <param name="settingObjectDto"></param>
        /// <returns></returns>
        public AutoTimeKeepingResult AutoTimeKeepingClockings(FingerPrintLogDto fingerPrintLog, Clocking clockingTarget,
           List<Clocking> beforeClockingsTarget, SettingObjectDto settingObjectDto)
        {
            #region Cập nhập ca hiện tại

            //Kiểm tra đã đồng bộ 
            if (clockingTarget.ClockingHistories.Any(history => history.AutoTimekeepingUid == fingerPrintLog.Uid))
            {
                var result = AutoTimeKeepingSuccess(fingerPrintLog, clockingTarget, true);
                return result;
            }

            //Cập nhập trang thái chấm công
            clockingTarget.UpdateClockingCheckedInDate(clockingTarget.StartTime, settingObjectDto);
            clockingTarget.UpdateClockingCheckedOutDate(fingerPrintLog.CheckDateTime, settingObjectDto);
            clockingTarget.UpdateClockingStatus((byte)ClockingStatuses.CheckedOut);

            //Ghi nhận lịch sử chấm công
            var clockingHistoryTarget = CreateClockingHistory(clockingTarget, fingerPrintLog.CheckDateTime,
                fingerPrintLog.Uid, (byte)TimeKeepingTypes.Automation, settingObjectDto);
            if (clockingHistoryTarget != null)
            {
                clockingTarget.ClockingHistories = clockingTarget.ClockingHistories ?? new List<ClockingHistory>();
                clockingTarget.ClockingHistories.Add(clockingHistoryTarget);
                _clockingsHistoryNeedAdd.Add(clockingHistoryTarget);
            }
            #endregion

            #region Cập nhập ca trước đó
            beforeClockingsTarget.ForEach(c =>
            {
                if (c.ClockingHistories.All(history => history.AutoTimekeepingUid != fingerPrintLog.Uid))
                {
                    if (c.CheckInDate == null) c.UpdateClockingCheckedInDate(c.StartTime, settingObjectDto);
                    if (c.CheckOutDate == null) c.UpdateClockingCheckedOutDate(c.EndTime, settingObjectDto);
                    c.UpdateClockingStatus((byte)ClockingStatuses.CheckedOut);

                    //Ghi nhận lịch sử chấm công
                    var beforeClockingHistory = CreateClockingHistory(c, fingerPrintLog.CheckDateTime, fingerPrintLog.Uid, (byte)TimeKeepingTypes.Automation, settingObjectDto);
                    if (beforeClockingHistory != null)
                    {
                        c.ClockingHistories = c.ClockingHistories ?? new List<ClockingHistory>();
                        c.ClockingHistories.Add(beforeClockingHistory);
                        _clockingsHistoryNeedAdd.Add(beforeClockingHistory);
                    }
                }
            });
            #endregion

            return AutoTimeKeepingSuccess(fingerPrintLog, clockingTarget);
        }


        /// <summary>
        /// Thực hiện chấm công tự động cho một dữ liệu chấm công
        /// </summary>
        /// <param name="fingerPrintLog"></param>
        /// <param name="clockingTarget"></param>
        /// <param name="settingObjectDto"></param>
        /// <returns></returns>
        public AutoTimeKeepingResult SingleAutoTimeKeeping(FingerPrintLogDto fingerPrintLog, Clocking clockingTarget, SettingObjectDto settingObjectDto)
        {
            try
            {
                #region Bước 1
                if (clockingTarget.ClockingHistories.Any(history => history.AutoTimekeepingUid == fingerPrintLog.Uid))
                {
                    var result = AutoTimeKeepingSuccess(fingerPrintLog, clockingTarget, true);
                    return result;
                }
                #endregion

                #region Bước 2: Xử lý ghi nhận thông tin chấm công cho ctlv
                /*
                 * a là thời điểm của lần chấm công (fingerPrinLog.checkDateTime)
                 * x và y tương ứng là thời gian bắt đầu và kết thúc của mỗi chi tiết làm việc (clocking.startTime và clocking.endTime)
                 * x' và y' tương ứng thời gian đã chấm công vào và ra của mỗi ca, x' hoặc y' có thể là null (chưa chấm công)
                 */

                var isCheckedIn = !IsNullOrDefaultDateTime(clockingTarget.CheckInDate);
                var isCheckedOut = !IsNullOrDefaultDateTime(clockingTarget.CheckOutDate);

                #region ctlv nghỉ

                if (clockingTarget.ClockingStatus == (byte)ClockingStatuses.WorkOff)
                {
                    var history = CreateClockingHistory(clockingTarget, fingerPrintLog.CheckDateTime, fingerPrintLog.Uid, (byte)TimeKeepingTypes.Fingerprint, settingObjectDto);
                    if (history != null)
                    {
                        _clockingsHistoryNeedAdd.Add(history);
                    }

                    fingerPrintLog.EmployeeId = clockingTarget.EmployeeId;
                    fingerPrintLog.Status = (int)FingerPrintLogStatus.Success;
                    return new AutoTimeKeepingResult()
                    {
                        FingerPrintLog = fingerPrintLog,
                        IsSuccess = true,
                        Message = "Thành công"
                    };
                }

                #endregion

                #region ctlv chưa chấm công vào và đã chấm công ra
                ClockingNotCheckInAndCheckedOut(isCheckedIn, isCheckedOut, clockingTarget, fingerPrintLog, settingObjectDto);
                #endregion

                #region ctlv chưa chấm công vào ra
                ClockingNotCheckInAndNotCheckout(isCheckedIn, isCheckedOut, clockingTarget, fingerPrintLog, settingObjectDto);
                #endregion

                #region ctlv đã chấm công vào và chưa chấm công ra
                // Nếu thời điểm chấm công =< giờ chấm công vào đang ghi nhận trên hệ thống thì cập nhật giờ vào hiện tại(x’ = a)
                // Nếu thời điểm chấm công > giờ chấm công vào đang ghi nhận trên hệ thống và =< giờ bắt đầu ca làm việc, thì hệ thống chỉ ghi nhận lịch sử chấm công mà không thay đổi giờ vào ra hiện tại
                // Else: coi đó là lần chấm công ra
                ClockingCheckedInAndNotCheckout(isCheckedIn, isCheckedOut, clockingTarget, fingerPrintLog, settingObjectDto);
                #endregion

                #region ctlv Đã chấm công vào và Đã chấm công ra
                // Nếu thời điểm chấm công<giờ chấm công vào đang ghi nhận trên hệ thống thì cập nhật giờ vào hiện tại (x’ = a).
                // Nếu thời điểm chấm công > giờ chấm công ra đang ghi nhận trên hệ thống thì cập nhật giờ ra hiện tại(y’ = a).
                // Nếu thời điểm chấm công nằm trong khoảng thời gian đã chấm công thì hệ thống chỉ ghi nhận lịch sử chấm công mà không thay đổi giờ vào ra hiện tại.
                ClockingCheckedInAndCheckedOut(isCheckedIn, isCheckedOut, clockingTarget, fingerPrintLog, settingObjectDto);
                #endregion

                #region  Ghi nhận lịch sử chấm công
                GenerateClockingHistories(clockingTarget, fingerPrintLog, settingObjectDto);

                return AutoTimeKeepingSuccess(fingerPrintLog, clockingTarget);

                #endregion


                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return AutoTimeKeepingFailed(fingerPrintLog, ex.Message);
            }
        }

        /// <summary>
        /// Tìm kiếm chi tiết làm việc hợp lệ ứng với một dữ liệu chấm công
        /// </summary>
        /// <param name="fingerPrintLog"></param>
        /// <param name="clockings"></param>
        /// <param name="employees"></param>
        /// <param name="shifts"></param>
        /// <returns></returns>
        public Clocking FindClockingForTimeKeeping(FingerPrintLogDto fingerPrintLog, IReadOnlyCollection<Clocking> clockings, List<FingerPrint> employees, List<Shift> shifts, bool isAutoTimekeepingCondition)
        {
            var employeeId = employees.FirstOrDefault(e => e.FingerCode == fingerPrintLog.FingerCode)?.EmployeeId;

            if (employeeId == null)
            {
                var ex = new Exception("Tài khoản chưa xác định nhân viên");
                throw ex;
            }

            if (clockings.Count == 0)
            {
                var ex = new Exception("Không có ca làm việc phù hợp");
                throw ex;
            }
            var employeeClocking = clockings
                .Where(c => c.EmployeeId == employeeId.Value).ToList();
            // các ctlv có thời điểm chấm công thuộc khoảng cho phép của ca
            // các ctlv thuộc các ca trên của nhân viên thực hiện chấm công
            var validClockings = (from c in employeeClocking
                                  let shift = shifts.FirstOrDefault(s => s.Id == c.ShiftId)
                                  where fingerPrintLog.CheckDateTime >=
                                        c.StartTime.AddHours(-GetHoursFromMinute(shift?.CheckInBefore, shift?.From))
                                  where fingerPrintLog.CheckDateTime <= c.EndTime.AddHours(GetHoursFromMinute(shift?.To, shift?.CheckOutAfter))
                                  select c).ToList();

            if (validClockings.Count == 0)
            {
                var ex = new Exception("Không có ca làm việc phù hợp");
                throw ex;
            }

            return FindClockingForTimeKeeping(fingerPrintLog, validClockings, isAutoTimekeepingCondition);
        }

        private Clocking FindClockingForTimeKeeping(FingerPrintLogDto fingerPrintLog, List<Clocking> validClockings, bool isAutoTimekeepingCondition)
        {
            // Th0. nếu chỉ có 1 ctlv thõa mãn => chọn ctlv đó
            if (validClockings.Count == 1) return validClockings.First();

            // Có 2 ca trùng giờ check in (giờ ra ca A là giờ vào ca B)
            var clockingInOutEqually = FindClockingForInOutEqually(fingerPrintLog, validClockings, isAutoTimekeepingCondition);
            if (clockingInOutEqually != null) return clockingInOutEqually;

            // Th1. a nằm trong khoảng thời gian của một chi tiết làm việc
            var case1Clockings = validClockings
                .Where(c => fingerPrintLog.CheckDateTime >= c.StartTime && fingerPrintLog.CheckDateTime <= c.EndTime)
                .ToList();
            if (case1Clockings.Count > 0)
            {
                // Th1.1. nếu chỉ có 1 ctlv => chọn ctlv đó
                if (case1Clockings.Count == 1) return case1Clockings.First();

                // Th1.2. nếu có nhiều ctlv thõa mãn => chọn ctlv theo dựa vào trạng thái
                return FindClockingForAutoTimeKeepingByStatus(case1Clockings);
            }

            // Th2. a không nằm trong khoảng thời gian làm việc của bất kì một ctlv nào
            // Th2.1. Nếu a nhỏ hơn thời gian bắt đầu của ctlv sớm nhất => chọn ctlv sớm nhất
            var minStartTime = validClockings.Select(c => c.StartTime).Min();
            var maxEndTime = validClockings.Select(c => c.EndTime).Max();
            if (fingerPrintLog.CheckDateTime < minStartTime)
            {
                return validClockings
                    .OrderBy(c => c.StartTime)
                    .First();
            }

            // Th2.2. nếu a lớn hơn thời gian kết thúc của ctlv muộn mất => chọn ctlv muộn nhất
            if (fingerPrintLog.CheckDateTime > maxEndTime)
            {
                return validClockings
                    .OrderByDescending(c => c.EndTime)
                    .First();
            }

            // Th2.3 nếu a lớn hơn thời điểm kết thúc của ctlv A và nhỏ hơn thời điểm kết thúc của ctlv B => chọn dựa vào trạng thái
            return FindClockingForAutoTimeKeepingByStatus(validClockings);
        }

        private Clocking FindClockingForInOutEqually(FingerPrintLogDto fingerPrintLog, List<Clocking> validClockings, bool isAutoTimekeepingCondition)
        {
            // Có 2 ca trùng giờ check in (giờ ra ca A là giờ vào ca B)
            // --> dữ liệu chấm công áp dụng cho vào ra ca A
            var validClockingInOutEqually = validClockings.Where(x => validClockings.Any(y => y.Id != x.Id && y.StartTime == x.EndTime)).ToList();
            if (validClockingInOutEqually?.Any() == true)
            {
                var clockingFirst = validClockingInOutEqually.OrderBy(x => x.StartTime).First();

                // Có ít nhất 2 ca trùng giờ, ca đầu chưa checkin, giờ check vân tay trước giờ ra ca đầu
                // --> Lấy ca sau
                if (
                    validClockingInOutEqually.Count > 1
                    && clockingFirst.CheckInDate == null
                    && clockingFirst.EndTime > fingerPrintLog.CheckDateTime)
                {
                    return validClockingInOutEqually.OrderBy(x => x.StartTime).Last();
                }

                // Giờ checkout chưa check hoặc bằng giờ check vân tay --> valid
                if (clockingFirst.CheckOutDate == null || clockingFirst.CheckOutDate == fingerPrintLog.CheckDateTime) return clockingFirst;

                // Nếu bật "Tự động ghi nhận thời gian chấm công" và ca trước đó đã check out
                // --> gán giờ vào mặc định cho ca sau
                // ex: Ca 1: 12-17h Ca 2: 17h-21h. Check out 2 lần 17h01, 17h02 --> 17h01 out ca 1, 17h00(giờ mặc định) in ca 2
                if (isAutoTimekeepingCondition) fingerPrintLog.IsSetDefaultCheckIn = true;
            }

            return null;
        }

        /// <summary>
        /// Tìm kiếm chi tiết làm việc hợp lệ ứng với dữ liệu chấm công trong trường hợp bật
        /// tính năng tự động chấm công
        /// </summary>
        /// <param name="checkDateTime"></param>
        /// <param name="clockingTarget"></param>
        /// <param name="clockings"> Danh sách clocking cần cập nhập</param>
        /// <param name="settingObjectDto"> Danh sách clocking cần cập nhập</param>
        /// <returns></returns>
        public List<Clocking> FindClockingForTimeKeepingMultiple(DateTime checkDateTime, Clocking clockingTarget, List<Clocking> clockings, SettingObjectDto settingObjectDto)
        {
            var clockingsForTimeKeeping = new List<Clocking>();
            if (clockingTarget == null) return new List<Clocking>();
            var totalMinutesLimitSetting = settingObjectDto.RangeShiftIsAutoTimekeepingMultipleHours * 60 +
                                           settingObjectDto.RangeShiftIsAutoTimekeepingMultipleMinutes;

            //Nếu ca phù hợp clockingTarget = Ca 1 khác:
            //*** Trạng thái 'Chưa vào - Chưa ra'
            //*** a > x1
            if (!(clockingTarget.CheckInDate == null && clockingTarget.CheckOutDate == null &&
                  checkDateTime > clockingTarget.StartTime.AddMinutes(1))) return new List<Clocking>();

            // Xác định khoảng thời gian nhằm khoanh vùng xử lý (nhằm giảm số lượng chi tiết làm việc lấy ra)
            var startTimeLimit = clockingTarget.StartTime.AddMinutes(1).AddDays(-settingObjectDto.MaxShiftIsAutoTimekeepingMultiple)
                .AddMinutes(-settingObjectDto.MaxShiftIsAutoTimekeepingMultiple * totalMinutesLimitSetting);
            var endTimeLimit = clockingTarget.StartTime.AddMinutes(1);

            //Lấy danh sách clocking theo một nhân viên
            var beforeClockings = clockings.Where(x => x.EmployeeId == clockingTarget.EmployeeId
                                                       && x.EndTime >= startTimeLimit
                                                       && x.StartTime <= endTimeLimit)
                .OrderByDescending(c => c.StartTime).Skip(0)
                .Take(settingObjectDto.MaxShiftIsAutoTimekeepingMultiple).ToList();

            //Lấy danh sách các clocking thỏa mãn điều kiện
            for (var i = 0; i < beforeClockings.Count; i++)
            {
                //Với ca trước đó trùng với ca 1 (tiếp tục xét tới ca tiếp theo)
                if (beforeClockings[i].Id == clockingTarget.Id) continue;

                if (beforeClockings[i - 1].StartTime.Subtract(beforeClockings[i].EndTime).TotalMinutes > totalMinutesLimitSetting)
                {
                    //Ca này không thỏa mãn (Xóa rỗng)
                    clockingsForTimeKeeping = new List<Clocking>();
                    break;
                }

                //TH1: Nếu ca 'Đã vào - Chưa ra' (không xét đến ca tiếp theo)
                if (beforeClockings[i].CheckInDate != null && beforeClockings[i].CheckOutDate == null)
                {
                    clockingsForTimeKeeping.Add(beforeClockings[i]);
                    break;
                }

                //TH2: Nếu ca 'Chưa vào - Chưa ra' (Xét tiếp đến ca tiếp theo)
                if (beforeClockings[i].CheckInDate == null && beforeClockings[i].CheckOutDate == null &&
                         i < (beforeClockings.Count - 1))
                {
                    clockingsForTimeKeeping.Add(beforeClockings[i]);
                    continue;
                }

                //Ca này không thỏa mãn (Xóa rỗng)
                clockingsForTimeKeeping = new List<Clocking>();
                break;

            }
            return clockingsForTimeKeeping;
        }

        /// <summary>
        /// Lấy tất cả các chi tiết làm việc có thể được chấm công ứng với danh sách dữ liệu chấm công nhận được
        /// </summary>
        /// <param name="fingerPrintLogs"></param>
        /// <param name="employees"></param>
        /// <param name="settingObjectDto"></param>
        /// <returns></returns>
        private async Task<List<Clocking>> GetClockingForTimeKeepingAsync(List<FingerPrintLogDto> fingerPrintLogs, List<FingerPrint> employees, SettingObjectDto settingObjectDto)
        {
            // Danh sách id máy chấm công của chi nhánh
            var fingerMachineIds =
                (await _fingerMachineWriteOnlyRepository.GetBySpecificationAsync(
                    new FindFingerMachineByBranchIdSpec(_authService.Context.BranchId))
                )
                .Select(machine => machine.MachineId)
                .ToList();

            // Chỉ lấy dữ liệu chấm công từ các máy chấm công của chi nhánh để xử lý
            // bỏ qua những dữ liệu chấm công từ các máy thuộc chi nhánh khác
            fingerPrintLogs = fingerPrintLogs
                .Where(log => fingerMachineIds.Any(machineId => machineId == log.MachineId))
                .ToList();

            if (fingerPrintLogs.Count == 0) return new List<Clocking>();

            /*
             * Xác định khoảng thời gian nhằm khoanh vùng xử lý (nhằm giảm số lượng chi tiết làm việc lấy ra)
             * Thời gian chấm ra trước không được sau giờ kết thúc ca quá 10 giờ (SRS 343)
             * Thời gian Chấm vào sau không được trước giờ bắt đầu ca quá 10 giờ (SRS 343)
             */
            var startTimeLimit = fingerPrintLogs.Select(x => x.CheckDateTime).Min()
                .AddHours(-Constant.MaximumCheckInCheckOutHours);
            var endTimeLimit = fingerPrintLogs.Select(x => x.CheckDateTime).Max()
                .AddHours(Constant.MaximumCheckInCheckOutHours);

            // Kiểm tra thỏa mãn tự động chấm công 2 ca liền nhau 
            if (IsAutoTimeKeepingMultiple(settingObjectDto))
            {
                var totalMinutesLimitSetting = settingObjectDto.RangeShiftIsAutoTimekeepingMultipleHours * 60 +
                                               settingObjectDto.RangeShiftIsAutoTimekeepingMultipleMinutes;
                //Tăng giới hạn tìm kiếm clocking thêm n ngày (Tối đa 3 ngày)
                startTimeLimit = startTimeLimit.AddDays(-settingObjectDto.MaxShiftIsAutoTimekeepingMultiple)
                    .AddMinutes(-settingObjectDto.MaxShiftIsAutoTimekeepingMultiple * totalMinutesLimitSetting);
            }

            // Danh sách nhân viên có ít nhất một dữ liệu chấm công nhận được
            var employeeIds = employees.Select(x => x.EmployeeId).ToList();

            /*
             * Tìm kiếm danh sách các clocking thõa mãn điều kiện xét
             * StartTimeClocking <= EndTimeLimit
             * EndTimeClocking >= StartTimeLimit
             */
            var clockings = await _clockingWriteOnlyRepository
                    .GetBySpecificationAsync(
                       spec: new FindClockingByBranchIdSpec(_authService.Context.BranchId)
                                .And(new FindClockingByEmployeeIdsSpec(employeeIds))
                                .And(new FindClockingByEndTimeGreaterThanOrEqualSpec(startTimeLimit))
                                .And(new FindClockingByStartTimeLessThanOrEqualSpec(endTimeLimit))
                                .Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void)),
                       include: "ClockingHistories"
                    );
            return clockings;
        }

        /// <summary>
        /// Chọn một ctlv để chấm công theo trạng thái nếu như các ctlv cùng thõa mãn các trường hợp thời gian
        /// </summary>
        /// <param name="validClockings"></param>
        /// <returns></returns>
        private Clocking FindClockingForAutoTimeKeepingByStatus(IReadOnlyCollection<Clocking> validClockings)
        {
            Clocking clockingTarget = null;

            // Nếu tồn tại ít nhất một ctlv (chi tiét làm việc) đã chấm công vào
            // Uu tiên ca làm việc của nhân viên có thời gian bắt đầu nhỏ nhất
            if (validClockings.Any(c => c.ClockingStatus == (byte)ClockingStatuses.CheckedIn))
            {
                var checkedInClockings = validClockings
                    .Where(c => c.ClockingStatus == (byte)ClockingStatuses.CheckedIn)
                    .OrderBy(c => c.StartTime)
                    .ToList();
                clockingTarget = checkedInClockings.First();

            }

            // Nếu không có ctlv nào đã chấm công vào và
            // Tồn tại ít nhât một ctlv chưa chấm công
            // Ưu tiên chọn ca làm việc có thời gian bắt đầu nhỏ nhất (nếu tồn tại nhiều ctlv chưa chấm công)
            else if (validClockings.Any(c => c.ClockingStatus == (byte)ClockingStatuses.Created))
            {
                var createdClockings = validClockings
                    .Where(c => c.ClockingStatus == (byte)ClockingStatuses.Created)
                    .OrderBy(c => c.StartTime)
                    .ToList();
                clockingTarget = createdClockings.First();
            }

            // Nếu không có ctlv nào đã chấm công hoặc chưa chấm công và
            // Tồn tại ít nhất một chi tiết làm việc đã chấm công ra (hoặc đã chấm công cả ra cả vào)
            // ưu tiên ca làm việc của nhân viên có thời gian bắt đầu nhỏ nhất
            else if (validClockings.Any(c => c.ClockingStatus == (byte)ClockingStatuses.CheckedOut))
            {
                var checkedOutClockings = validClockings
                    .Where(c => c.ClockingStatus == (byte)ClockingStatuses.CheckedOut)
                    .OrderBy(c => c.StartTime)
                    .ToList();
                clockingTarget = checkedOutClockings.First();
            }

            // Nếu tất cả ctlv đều ở trạng thái nghỉ
            else if (validClockings.Any(c => c.ClockingStatus == (byte)ClockingStatuses.WorkOff))
            {
                var checkedOutClockings = validClockings
                    .Where(c => c.ClockingStatus == (byte)ClockingStatuses.WorkOff)
                    .OrderBy(c => c.StartTime)
                    .ToList();
                clockingTarget = checkedOutClockings.First();
            }

            if (clockingTarget == null)
            {
                var ex = new Exception("Không có ca làm việc phù hợp");
                throw ex;
            }

            return clockingTarget;
        }

        /// <summary>
        /// Ghi nhận lịch sử chấm công tự động
        /// </summary>
        /// <param name="clocking"></param>
        /// <param name="checkTime"></param>
        /// <param name="autoTimekeepingUid"></param>
        /// <param name="timeKeepingTypes"></param>
        /// <param name="settingObjectDto"></param>
        /// <returns></returns>
        private ClockingHistory CreateClockingHistory(Clocking clocking, DateTime? checkTime, string autoTimekeepingUid,
            byte timeKeepingTypes,
            SettingObjectDto settingObjectDto)
        {
            var leaveOfAbsence = clocking.AbsenceType != null;
            var newClockingHistory = ClockingHistory.GenerateClockingHistory(
                clocking.Id,
                clocking.BranchId,
                clocking.TenantId,
                clocking.CheckInDate,
                clocking.CheckOutDate,
                clocking.StartTime,
                clocking.EndTime,
                timeKeepingTypes,
                settingObjectDto,
                clocking.ClockingStatus,
                clocking.TimeIsLate,
                clocking.OverTimeBeforeShiftWork,
                clocking.TimeIsLeaveWorkEarly,
                clocking.OverTimeAfterShiftWork,
                clocking.AbsenceType,
                clocking.AbsenceType,
                leaveOfAbsence,
                clocking.ShiftId,
                clocking.EmployeeId,
                clocking.ClockingHistories?.OrderByDescending(ch => ch.CreatedDate).FirstOrDefault(),
                checkTime,
                autoTimekeepingUid
            );

            return newClockingHistory;
        }

        /// <summary>
        /// Kết quả trả về nếu chấm công tự động thành công
        /// </summary>
        /// <param name="log"></param>
        /// <param name="clocking"></param>
        /// <param name="isExist"></param>
        /// <returns></returns>
        private static AutoTimeKeepingResult AutoTimeKeepingSuccess(FingerPrintLogDto log, Clocking clocking, bool isExist = false)
        {
            log.EmployeeId = clocking.EmployeeId;
            log.Status = (int)FingerPrintLogStatus.Success;
            return new AutoTimeKeepingResult()
            {
                FingerPrintLog = log,
                IsSuccess = true,
                Message = "Thành công",
                IsExist = isExist
            };

        }

        /// <summary>
        /// kết quả trả về nếu chấm công tự động không thành công
        /// </summary>
        /// <param name="log"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static AutoTimeKeepingResult AutoTimeKeepingFailed(FingerPrintLogDto log, string message)
        {
            log.Status = (int)FingerPrintLogStatus.Faild;
            return new AutoTimeKeepingResult()
            {
                FingerPrintLog = log,
                IsSuccess = false,
                Message = message
            };
        }

        public static bool IsNullOrDefaultDateTime(DateTime? dateTime) =>
            dateTime == null || dateTime == default(DateTime);

        /// <summary>
        /// Thêm mới hoặc cập nhập clocking 
        /// </summary>
        /// <param name="newClockings"></param>
        private void AddOrUpdateClockingAutoKeeping(List<Clocking> newClockings)
        {
            newClockings.ForEach(c =>
            {
                var clockingUpdate = _clockingsNeedUpdate.FirstOrDefault(x => x.Id == c.Id);
                if (clockingUpdate == null)
                {
                    _clockingsNeedUpdate.Add(c);
                }
            });
        }

        /// <summary>
        /// Kiểm tra thỏa mãn chấm công tự động cho nhiều ca liền nhau (SRS: #3021)
        /// </summary>
        /// <param name="settingObjectDto"></param>
        /// <returns></returns>
        private bool IsAutoTimeKeepingMultiple(SettingObjectDto settingObjectDto)
        {
            if (!settingObjectDto.IsAutoTimekeepingMultiple || settingObjectDto.AllowAutoKeeping) return false;

            if (settingObjectDto.MaxShiftIsAutoTimekeepingMultiple < Constant.MinimumShiftIsAutoTimekeeping ||
                settingObjectDto.MaxShiftIsAutoTimekeepingMultiple > Constant.MaximumShiftIsAutoTimekeeping)
                return false;

            if (settingObjectDto.RangeShiftIsAutoTimekeepingMultipleHours > Constant.MaximumRangeShiftIsAutoTimekeepingHours
                || settingObjectDto.RangeShiftIsAutoTimekeepingMultipleMinutes > Constant.MaximumRangeShiftIsAutoTimekeepingMinutes)
                return false;

            return true;
        }

        /// <summary>
        /// ctlv chưa chấm công vào và đã chấm công ra
        /// </summary>
        /// <param name="isCheckedIn"></param>
        /// <param name="isCheckedOut"></param>
        /// <param name="clockingTarget"></param>
        /// <param name="fingerPrintLog"></param>
        /// <param name="settingObjectDto"></param>
        private void ClockingNotCheckInAndCheckedOut(bool isCheckedIn, bool isCheckedOut, Clocking clockingTarget, FingerPrintLogDto fingerPrintLog, SettingObjectDto settingObjectDto)
        {
            //- Nếu thời điểm chấm công > giờ chấm công ra đang ghi nhận trên hệ thống thì cập nhật giờ ra hiện tại(y’ = a)
            //- Nếu thời điểm chấm công >= giờ kết thúc ca và =< giờ chấm công ra đang ghi nhận trên hệ thống, thì hệ thống chỉ ghi nhận lịch sử chấm công
            //- else: coi đó là lần chấm công vào

            if (isCheckedIn || !isCheckedOut) return;

            // nếu a > y'
            if (fingerPrintLog.CheckDateTime > clockingTarget.CheckOutDate)
            {
                // cập nhật giờ ra, tính toán lại giờ làm thêm, hoặc về sớm.
                clockingTarget.UpdateClockingCheckedOutDate(fingerPrintLog.CheckDateTime, settingObjectDto);
            }
            // nếu a <= y' và a <= y
            else if (fingerPrintLog.CheckDateTime < clockingTarget.CheckOutDate &&
                     fingerPrintLog.CheckDateTime < clockingTarget.EndTime)
            {
                // Cập nhật giờ vào, tính toán lại giờ làm thêm trước ca hoặc đi muộn theo rule mặc định
                clockingTarget.UpdateClockingCheckedInDate(fingerPrintLog.CheckDateTime, settingObjectDto);
            }
        }

        /// <summary>
        /// ctlv chưa chấm công vào ra
        /// </summary>
        /// <param name="isCheckedIn"></param>
        /// <param name="isCheckedOut"></param>
        /// <param name="clockingTarget"></param>
        /// <param name="fingerPrintLog"></param>
        /// <param name="settingObjectDto"></param>
        private void ClockingNotCheckInAndNotCheckout(bool isCheckedIn, bool isCheckedOut, Clocking clockingTarget, FingerPrintLogDto fingerPrintLog, SettingObjectDto settingObjectDto)
        {
            // - Thời điểm chấm công < thời gian kết thúc ca: coi đó là thời điểm chấm công vào.
            // - Else: coi đó là lần chấm công ra
            if (isCheckedIn || isCheckedOut) return;
            var isOutTime = CheckDateTimeIsOutTime(fingerPrintLog.CheckDateTime, clockingTarget.EndTime, clockingTarget.StartTime);
            if (isOutTime)
            {
                // Cập nhật giờ ra, tính toán lại giờ làm thêm sau ca hoặc về sớm theo rule mặc định.
                clockingTarget.UpdateClockingCheckedOutDate(fingerPrintLog.CheckDateTime, settingObjectDto);

                // Chuyển trạng thái ca làm việc thành ‘Đã chấm công ra’.
                clockingTarget.UpdateClockingStatus((byte)ClockingStatuses.CheckedOut);
            }
            else
            {
                // Cập nhật giờ vào, tính toán lại giờ làm thêm trước ca hoặc đi muộn theo rule mặc định,
                var checkInDate = fingerPrintLog.IsSetDefaultCheckIn ? clockingTarget.StartTime : fingerPrintLog.CheckDateTime;
                clockingTarget.UpdateClockingCheckedInDate(checkInDate, settingObjectDto);

                // Chuyển trạng thái Ca làm việc thành ‘Đã chấm công vào’,
                clockingTarget.UpdateClockingStatus((byte)ClockingStatuses.CheckedIn);
            }
        }

        /// <summary>
        /// ctlv đã chấm công vào và chưa chấm công ra
        /// </summary>
        /// <param name="isCheckedIn"></param>
        /// <param name="isCheckedOut"></param>
        /// <param name="clockingTarget"></param>
        /// <param name="fingerPrintLog"></param>
        /// <param name="settingObjectDto"></param>
        private void ClockingCheckedInAndNotCheckout(bool isCheckedIn, bool isCheckedOut, Clocking clockingTarget, FingerPrintLogDto fingerPrintLog, SettingObjectDto settingObjectDto)
        {
            if (!isCheckedIn || isCheckedOut) return;
            // nếu a <= x'
            if (fingerPrintLog.CheckDateTime <= clockingTarget.CheckInDate)
            {
                // Cập nhật giờ vào, tính toán lại giờ làm thêm trước ca hoặc đi muộn theo rule mặc định.
                clockingTarget.UpdateClockingCheckedInDate(fingerPrintLog.CheckDateTime, settingObjectDto);
            }

            // Nếu x' <= x < a < y hoặc x <= x' < a < y
            else if (fingerPrintLog.CheckDateTime < clockingTarget.EndTime &&
                     fingerPrintLog.CheckDateTime > clockingTarget.CheckInDate &&
                     fingerPrintLog.CheckDateTime > clockingTarget.StartTime.AddMinutes(1)
            )
            {
                var lastHistory = clockingTarget.ClockingHistories.OrderByDescending(history => history.CreatedDate).FirstOrDefault();
                if (lastHistory?.CheckedInDate != null &&
                    Math.Abs(lastHistory.CheckedInDate.Value.Subtract(fingerPrintLog.CheckDateTime).TotalMinutes) > 2)
                {
                    // Cập nhật giờ ra, tính toán lại giờ làm thêm sau ca hoặc về sớm theo rule mặc định.
                    clockingTarget.UpdateClockingCheckedOutDate(fingerPrintLog.CheckDateTime, settingObjectDto);

                    // Chuyển trạng thái ca làm việc thành ‘Đã chấm công ra’.
                    clockingTarget.UpdateClockingStatus((byte)ClockingStatuses.CheckedOut);
                }
            }
            // nếu a >= y
            else if (fingerPrintLog.CheckDateTime >= clockingTarget.EndTime)
            {
                // Cập nhật giờ ra, tính toán lại giờ làm thêm sau ca hoặc về sớm theo rule mặc định.
                clockingTarget.UpdateClockingCheckedOutDate(fingerPrintLog.CheckDateTime, settingObjectDto);

                // Chuyển trạng thái ca làm việc thành ‘Đã chấm công ra’.
                clockingTarget.UpdateClockingStatus((byte)ClockingStatuses.CheckedOut);
            }
        }

        /// <summary>
        /// ctlv Đã chấm công vào và Đã chấm công ra
        /// </summary>
        /// <param name="isCheckedIn"></param>
        /// <param name="isCheckedOut"></param>
        /// <param name="clockingTarget"></param>
        /// <param name="fingerPrintLog"></param>
        /// <param name="settingObjectDto"></param>
        private void ClockingCheckedInAndCheckedOut(bool isCheckedIn, bool isCheckedOut, Clocking clockingTarget, FingerPrintLogDto fingerPrintLog, SettingObjectDto settingObjectDto)
        {
            if (!isCheckedIn || !isCheckedOut) return;

            if (fingerPrintLog.CheckDateTime < clockingTarget.CheckInDate)
            {
                // Cập nhật giờ vào, tính toán lại giờ làm thêm trước ca hoặc đi muộn theo rule mặc định.
                clockingTarget.UpdateClockingCheckedInDate(fingerPrintLog.CheckDateTime, settingObjectDto);
            }
            else if (fingerPrintLog.CheckDateTime > clockingTarget.CheckOutDate)
            {
                // Cập nhật giờ ra, tính toán lại giờ làm thêm sau ca hoặc về sớm theo rule mặc định.
                clockingTarget.UpdateClockingCheckedOutDate(fingerPrintLog.CheckDateTime, settingObjectDto);
            }
        }

        /// <summary>
        /// Ghi nhận lịch sử chấm công
        /// </summary>
        /// <param name="clockingTarget"></param>
        /// <param name="fingerPrintLog"></param>
        /// <param name="settingObjectDto"></param>
        private void GenerateClockingHistories(Clocking clockingTarget, FingerPrintLogDto fingerPrintLog, SettingObjectDto settingObjectDto)
        {
            var clockingHistory = CreateClockingHistory(clockingTarget, fingerPrintLog.CheckDateTime, fingerPrintLog.Uid, (byte)TimeKeepingTypes.Fingerprint, settingObjectDto);
            if (clockingHistory != null)
            {
                clockingTarget.ClockingHistories = clockingTarget.ClockingHistories ?? new List<ClockingHistory>();
                clockingTarget.ClockingHistories.Add(clockingHistory);
                _clockingsHistoryNeedAdd.Add(clockingHistory);
            }
        }
    }
}
