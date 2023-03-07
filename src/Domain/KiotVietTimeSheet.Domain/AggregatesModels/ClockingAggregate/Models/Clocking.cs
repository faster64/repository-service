using System;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models
{
    public class Clocking : BaseEntity,
        IAggregateRoot,
        IEntityIdlong,
        ITenantId,
        IBranchId,
        ICreatedBy,
        ICreatedDate,
        IModifiedBy,
        IModifiedDate,
        ISoftDelete
    {
        #region Properties
        /// <summary>
        /// Id chi tiết làm việc
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Id lịch làm việc
        /// </summary>
        public long TimeSheetId { get; protected set; }

        /// <summary>
        /// Id ca làm việc
        /// </summary>
        public long ShiftId { get; protected set; }

        /// <summary>
        /// Id nhân viên được gán với chi tiết làm việc (nhân viên được gán ban đầu)
        /// </summary>
        public long EmployeeId { get; protected set; }

        /// <summary>
        /// Id nhân viên thực hiện làm việc trên chi tiết làm việc (trường hợp nhân viên làm thay, đổi ca...)
        /// </summary>
        public long WorkById { get; protected set; }

        /// <summary>
        /// Trạng thái chi tiết làm việc <see cref="ClockingStatuses"/>
        /// </summary>
        public byte ClockingStatus { get; protected set; }

        /// <summary>
        /// Thời gian bắt đầu chi tiết làm việc
        /// </summary>
        public DateTime StartTime { get; protected set; }

        /// <summary>
        /// Thời điểm kết thúc chi tiết làm việc
        /// </summary>
        public DateTime EndTime { get; protected set; }

        /// <summary>
        /// Ghi chú chi tiết làm việc
        /// </summary>
        public string Note { get; protected set; }

        /// <summary>
        /// Id cửa hàng/ doanh nghiệp
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// Id chi nhánh của chi tiết làm việc
        /// </summary>
        public int BranchId { get; set; }

        /// <summary>
        /// Id người tạo ra chi tiết làm việc
        /// </summary>
        public long CreatedBy { get; set; }

        /// <summary>
        /// Thởi điểm tạo chi tiết làm việc
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Id người sửa đổi cuối cùng chi tiết làm việc
        /// </summary>
        public long? ModifiedBy { get; set; }

        /// <summary>
        /// Thời điểm cuổi cùng sửa đổi chi tiết làm việc
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Trạng thái xóa mềm chi tiết làm việc
        /// true - chi tiết làm việc đã bị xóa
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Id người xóa chi tiết làm việc
        /// </summary>
        public long? DeletedBy { get; set; }

        /// <summary>
        /// Thời điểm xóa chi tiết làm việc
        /// </summary>
        public DateTime? DeletedDate { get; set; }

        /// <summary>
        /// Thời điểm chấm công vào
        /// </summary>
        public DateTime? CheckInDate { get; set; }

        /// <summary>
        /// Thời điểm chấm công ra
        /// </summary>
        public DateTime? CheckOutDate { get; set; }

        /// <summary>
        /// Thời gian đi muộn
        /// </summary>
        public int TimeIsLate { get; protected set; }

        /// <summary>
        /// Thời gian làm thêm trước ca
        /// </summary>
        public int OverTimeBeforeShiftWork { get; protected set; }

        /// <summary>
        /// Thời gian về sớm
        /// </summary>
        public int TimeIsLeaveWorkEarly { get; protected set; }

        /// <summary>
        /// Thời gian làm thêm sau ca
        /// </summary>
        public int OverTimeAfterShiftWork { get; protected set; }

        /// <summary>
        /// Trạng thái nghỉ <see cref="AbsenceTypes"/>
        /// null = k nghỉ
        /// </summary>
        public byte? AbsenceType { get; set; }

        /// <summary>
        /// Trạng thái trả lương <see cref="ClockingPaymentStatuses"/>
        /// </summary>
        public byte ClockingPaymentStatus { get; set; }

        /// <summary>
        /// Lịch sử chám công của chi tiết làm việc <see cref="ClockingHistory"/>
        /// </summary>
        public List<ClockingHistory> ClockingHistories { get; set; }

        /// <summary>
        /// Người thực hiện chi tiết làm việc <see cref="Employee"/>
        /// </summary>
        public Employee WorkBy { get; set; }

        /// <summary>
        /// Người được gán với chi tiết làm việc lúc khởi tạo <see cref="Employee"/>
        /// </summary>

        public Employee Employee { get; set; }

        /// <summary>
        /// Lịch sử vi phạm <see cref="ClockingPenalize"/>
        /// </summary>
        public List<ClockingPenalize> ClockingPenalizes { get; set; }

        #endregion 

        #region Constructor
        public Clocking()
        {
            IsDeleted = false;
        }

        /// <summary>
        /// JsonConstructor
        /// </summary>
        [JsonConstructor]
        public Clocking(long id, long timeSheetId, long shiftId, long employeeId, long workById, byte clockingStatus, DateTime startTime, DateTime endTime, string note, int tenantId, int branchId, long createdBy, DateTime createdDate, long? modifiedBy, DateTime? modifiedDate, bool isDeleted, long? deletedBy, DateTime? deletedDate, DateTime? checkInDate, DateTime? checkOutDate, int timeIsLate, int overTimeBeforeShiftWork, int timeIsLeaveWorkEarly, int overTimeAfterShiftWork, byte? absenceType, List<ClockingHistory> clockingHistories, byte clockingPaymentStatus, Employee workBy, Employee employee, TimeSheet timeSheet)
        {
            Id = id;
            TimeSheetId = timeSheetId;
            ShiftId = shiftId;
            EmployeeId = employeeId;
            WorkById = workById;
            ClockingStatus = clockingStatus;
            StartTime = startTime;
            EndTime = endTime;
            Note = note;
            TenantId = tenantId;
            BranchId = branchId;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
            ModifiedBy = modifiedBy;
            ModifiedDate = modifiedDate;
            IsDeleted = isDeleted;
            DeletedBy = deletedBy;
            DeletedDate = deletedDate;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            TimeIsLate = timeIsLate;
            OverTimeBeforeShiftWork = overTimeBeforeShiftWork;
            TimeIsLeaveWorkEarly = timeIsLeaveWorkEarly;
            OverTimeAfterShiftWork = overTimeAfterShiftWork;
            AbsenceType = absenceType;
            ClockingHistories = clockingHistories;
            ClockingPaymentStatus = clockingPaymentStatus;
            WorkBy = workBy;
            Employee = employee;
        }

        /// <summary>
        /// Create clocking
        /// </summary>
        /// <param name="timeSheetId"></param>
        /// <param name="shiftId"></param>
        /// <param name="employeeId"></param>
        /// <param name="workById"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="tenantId"></param>
        /// <param name="branchId"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public Clocking(
            long timeSheetId,
            long shiftId,
            long employeeId,
            long workById,
            DateTime startTime,
            DateTime endTime,
            int tenantId,
            int branchId,
            string note)
        {

            TimeSheetId = timeSheetId;
            ShiftId = shiftId;
            EmployeeId = employeeId;
            WorkById = workById;
            ClockingStatus = (byte)ClockingStatuses.Created;
            StartTime = startTime;
            EndTime = endTime;
            TenantId = tenantId;
            BranchId = branchId;
            Note = note;
            ClockingPaymentStatus = (byte)ClockingPaymentStatuses.UnPaid;
        }

        /// <summary>
        /// Tạo clocking khi đổi người làm thay
        /// </summary>
        /// <param name="timeSheetId"></param>
        /// <param name="shift"></param>
        /// <param name="employeeId"></param>
        /// <param name="workById"></param>
        /// <param name="tenantId"></param>
        /// <param name="branchId"></param>
        /// <param name="note"></param>
        /// <param name="startTime"></param>
        public Clocking(
            long timeSheetId,
            Shift shift,
            long employeeId,
            long workById,
            int tenantId,
            int branchId,
            string note,
            DateTime startTime
            )
        {

            TimeSheetId = timeSheetId;
            ShiftId = shift.Id;
            EmployeeId = employeeId;
            WorkById = workById;
            ClockingStatus = (byte)ClockingStatuses.Created;
            SetTimeByShift(startTime, shift);
            TenantId = tenantId;
            BranchId = branchId;
            Note = note;
            ClockingPaymentStatus = (byte)ClockingPaymentStatuses.UnPaid;
        }

        // Only copy primitive values
        public Clocking(Clocking clocking)
        {
            Id = clocking.Id;
            TimeSheetId = clocking.TimeSheetId;
            ShiftId = clocking.ShiftId;
            EmployeeId = clocking.EmployeeId;
            WorkById = clocking.WorkById;
            ClockingStatus = clocking.ClockingStatus;
            StartTime = clocking.StartTime;
            EndTime = clocking.EndTime;
            Note = clocking.Note;
            TenantId = clocking.TenantId;
            BranchId = clocking.BranchId;
            CreatedBy = clocking.CreatedBy;
            CreatedDate = clocking.CreatedDate;
            ModifiedBy = clocking.ModifiedBy;
            ModifiedDate = clocking.ModifiedDate;
            IsDeleted = clocking.IsDeleted;
            DeletedBy = clocking.DeletedBy;
            DeletedDate = clocking.DeletedDate;
            CheckInDate = clocking.CheckInDate;
            CheckOutDate = clocking.CheckOutDate;
            TimeIsLate = clocking.TimeIsLate;
            OverTimeBeforeShiftWork = clocking.OverTimeBeforeShiftWork;
            TimeIsLeaveWorkEarly = clocking.TimeIsLeaveWorkEarly;
            OverTimeAfterShiftWork = clocking.OverTimeAfterShiftWork;
            AbsenceType = clocking.AbsenceType;
            ClockingPaymentStatus = clocking.ClockingPaymentStatus;
            if (clocking.DomainEvents != null)
            {
                foreach (var domainEvent in clocking.DomainEvents)
                {
                    AddDomainEvent(domainEvent);
                }
            }
        }

        #endregion

        #region public method

        /// <summary>
        /// Tạo danh sách chi tiết làm việc cho một lịch làm việc
        /// </summary>
        /// <param name="timeSheet">Lịch làm việc</param>
        /// <param name="clockingsOnShift"></param>
        /// <param name="shift">Ca làm việc</param>
        /// <param name="forDate">Ngày tạo chi tiết làm việc</param>
        /// <returns></returns>
        public static Clocking CreateForTimeSheet(TimeSheet timeSheet, List<Clocking> clockingsOnShift, Shift shift, DateTime forDate)
        {
            var startTime = forDate.Date.AddMinutes(shift.From);
            var endTime = forDate.Date.AddMinutes(shift.To);
            if (shift.From > shift.To) endTime = endTime.AddDays(1);

            var clockingOverlap = clockingsOnShift?.FirstOrDefault(c =>
                c.StartTime.Date == startTime.Date && c.EndTime.Date == endTime.Date
                                                   && c.ClockingStatus != (byte)ClockingStatuses.Void && !c.IsDeleted);
            // Các clockings có trạng thái clocking hủy và chưa chấm công sẽ được tạo lại, các loại khác sẽ k tác động gì
            if (clockingOverlap != null)
            {
                // không thể tạo chi tiết làm việc mới, trả lại chi tiết làm việc trùng đã tồn tại
                return clockingOverlap;
            }

            // tạo chi tiết làm việc mới khi không có tri tiêt làm việc nào trùng trên ca
            var clocking = new Clocking(
                // Sử dụng temporary id cho trường hợp thêm mới lịch làm việc
                timeSheet.Id > 0 ? timeSheet.Id : timeSheet.TemporaryId,
                shift.Id,
                timeSheet.EmployeeId,
                timeSheet.EmployeeId,
                startTime,
                endTime,
                timeSheet.TenantId,
                timeSheet.BranchId,
                null);
            return clocking;
        }

        public void UpdateClockingAfterCreateClockingHistory(ClockingHistory clockingHistory, Clocking oldClocking)
        {
            if (clockingHistory != null)
            {
                UpdateClockingStatusAndAbsenceType(clockingHistory.ClockingStatus, clockingHistory.AbsenceType);
                UpdateTimeColumnsFromClockingHistory(
                    clockingHistory.TimeIsLate,
                    clockingHistory.OverTimeBeforeShiftWork,
                    clockingHistory.TimeIsLeaveWorkEarly,
                    clockingHistory.OverTimeAfterShiftWork,
                    clockingHistory.CheckedInDate,
                    clockingHistory.CheckedOutDate
                );
            }

            AddDomainEvent(new UpdatedClockingEvent(oldClocking, this));
        }

        public void UpdateWithoutAddDomainEvent(
            long shiftId,
            DateTime startTime,
            DateTime endTime,
            string note,
            Shift shift
            )
        {
            var isChangedShift = ShiftId != shiftId;
            StartTime = startTime;
            EndTime = endTime;
            ShiftId = shiftId;
            Note = note;
            if (isChangedShift)
            {
                SetTimeByShift(startTime, shift);
            }
        }

        public void UpdateClockingStatusAndAbsenceType(byte clockingStatus, byte? absenceType = null)
        {
            ClockingStatus = clockingStatus;
            AbsenceType = absenceType;
        }

        public void UpdateClockingStatus(byte clockingStatus)
        {
            ClockingStatus = clockingStatus;
        }

        public void UpdateClockingTimeSheetId(long timeSheetId)
        {
            TimeSheetId = timeSheetId;
        }

        public void UpdateClockingShiftAndDateTime(long shiftId, DateTime startTime, Shift shift)
        {
            ShiftId = shiftId;
            SetTimeByShift(startTime, shift);
        }

        public void UpdateClockingShiftAndDateTime(long shiftId, int branchId, DateTime startTime, Shift shift)
        {
            ShiftId = shiftId;
            BranchId = branchId;
            SetTimeByShift(startTime, shift);
        }

        public void UpdateClockingShiftAndCheckInCheckOut(long shiftId, DateTime? checkInDate, DateTime? checkOutDate)
        {
            ShiftId = shiftId;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
        }

        public void UpdateClockingShiftAndStatus(long shiftId, byte clockingStatus)
        {
            ShiftId = shiftId;
            ClockingStatus = clockingStatus;
        }

        public void UpdateClockingCheckedInDate(DateTime? checkInDate, SettingsToObject settings)
        {
            CheckInDate = checkInDate;
            if (checkInDate == null)
            {
                TimeIsLate = 0;
                OverTimeAfterShiftWork = 0;
            }
            else
            {
                if (settings.IsAutoCalcLateTime)
                {
                    var calcStartTime = StartTime.AddMinutes(settings.LateTime);
                    var isStartLate = calcStartTime < checkInDate;
                    TimeIsLate = (int)Math.Round(isStartLate ? (checkInDate - calcStartTime).Value.TotalMinutes : 0);
                }
                else
                {
                    TimeIsLate = 0;
                }

                if (settings.IsAutoCalcEarlyTimeOT)
                {
                    var calcStartTimeOT = StartTime.AddMinutes(-settings.EarlyTimeOT);
                    var isStartEarly = calcStartTimeOT > checkInDate;
                    OverTimeBeforeShiftWork = (int)Math.Round(isStartEarly ? (calcStartTimeOT - checkInDate).Value.TotalMinutes : 0);
                }
                else
                {
                    OverTimeBeforeShiftWork = 0;
                }
            }
        }

        public void UpdateClockingCheckedOutDate(DateTime? checkOutDate, SettingsToObject settings)
        {
            CheckOutDate = checkOutDate;
            if (checkOutDate == null)
            {
                TimeIsLeaveWorkEarly = 0;
                OverTimeAfterShiftWork = 0;
            }
            else
            {
                if (settings.IsAutoCalcEarlyTime)
                {
                    var calcEndTimeEarly = EndTime.AddMinutes(-settings.EarlyTime);
                    var isLeaveEarly = calcEndTimeEarly > checkOutDate;
                    TimeIsLeaveWorkEarly = (int)Math.Round(isLeaveEarly ? (calcEndTimeEarly - checkOutDate).Value.TotalMinutes : 0);
                }
                else
                {
                    TimeIsLeaveWorkEarly = 0;
                }

                if (settings.IsAutoCalcLateTimeOT)
                {
                    var calcEndTimeOT = EndTime.AddMinutes(settings.LateTimeOT);
                    var isLeaveLate = calcEndTimeOT < checkOutDate;
                    OverTimeAfterShiftWork = (int)Math.Round(isLeaveLate ? (checkOutDate - calcEndTimeOT).Value.TotalMinutes : 0);
                }
                else
                {
                    OverTimeAfterShiftWork = 0;
                }
            }

        }

        /// <summary>
        /// Thực hiện hủy bỏ chi tiết làm việc
        /// </summary>
        /// <returns></returns>
        public void Reject()
        {
            ClockingStatus = (byte)ClockingStatuses.Void;
        }

        /// <summary>
        /// Thực hiện xóa mềm 1 chi tiết làm việc
        /// </summary>
        /// <returns></returns>
        public void Delete()
        {
            IsDeleted = true;
        }

        /// <summary>
        /// Shadow copy một chi tiết làm việc
        /// </summary>
        /// <returns></returns>
        public Clocking CreateCopy()
        {
            return new Clocking()
            {
                Id = Id,
                DeletedDate = DeletedDate,
                BranchId = BranchId,
                ClockingHistories = ClockingHistories,
                ClockingStatus = ClockingStatus,
                CheckInDate = CheckInDate,
                CheckOutDate = CheckOutDate,
                CreatedBy = CreatedBy,
                CreatedDate = CreatedDate,
                DeletedBy = DeletedBy,
                EmployeeId = EmployeeId,
                EndTime = EndTime,
                IsDeleted = IsDeleted,
                ModifiedBy = ModifiedBy,
                ModifiedDate = ModifiedDate,
                Note = Note,
                ShiftId = ShiftId,
                StartTime = StartTime,
                TenantId = TenantId,
                TimeSheetId = TimeSheetId,
                WorkBy = WorkBy,
                WorkById = WorkById,
                AbsenceType = AbsenceType,
                ClockingPaymentStatus = ClockingPaymentStatus,
                TimeIsLate = TimeIsLate,
                TimeIsLeaveWorkEarly = TimeIsLeaveWorkEarly,
                OverTimeAfterShiftWork = OverTimeAfterShiftWork,
                OverTimeBeforeShiftWork = OverTimeBeforeShiftWork
            };
        }

        public void UpdateTimeColumnsFromClockingHistory(
            int timeIsLate,
            int overTimeBeforeShiftWork,
            int timeIsLeaveWorkEarly,
            int overTimeAfterShiftWork,
            DateTime? checkInDate = null,
            DateTime? checkOutDate = null
            )
        {
            TimeIsLate = timeIsLate;
            OverTimeBeforeShiftWork = overTimeBeforeShiftWork;
            TimeIsLeaveWorkEarly = timeIsLeaveWorkEarly;
            OverTimeAfterShiftWork = overTimeAfterShiftWork;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
        }

        public void SwapShiftWithoutAddDomainEvent(
            long shiftId,
            DateTime startTime,
            DateTime endTime
        )
        {
            ShiftId = shiftId;
            StartTime = startTime;
            EndTime = endTime;
        }

        public void SwapShift(
            Clocking oldTarget,
            Clocking oldSource
        )
        {
            AddDomainEvent(new SwappedClockingEvent(oldSource, oldTarget));
            SwapShiftWithoutAddDomainEvent(oldTarget.ShiftId, oldTarget.StartTime, oldTarget.EndTime);
        }

        public bool IsDataNotChanged(Clocking clocking, bool leaveOfAbsence = false)
        {
            if (clocking == null) return false;

            if (leaveOfAbsence)
            {
                if (HasLeaveOfAbsence(clocking)) return true;
            }
            else
                if (clocking.CheckOutDate == null)
                    return CheckOutDate == null && AbsenceType != (byte)AbsenceTypes.AuthorisedAbsence;

            return 
                   IsDeleted == clocking.IsDeleted &&
                   ClockingStatus == clocking.ClockingStatus &&
                   CheckInDate == clocking.CheckInDate &&
                   CheckOutDate == clocking.CheckOutDate &&
                   AbsenceType == clocking.AbsenceType &&
                   OverTimeAfterShiftWork == clocking.OverTimeAfterShiftWork &&
                   OverTimeBeforeShiftWork == clocking.OverTimeBeforeShiftWork &&
                   TimeIsLate == clocking.TimeIsLate &&
                   TimeIsLeaveWorkEarly == clocking.TimeIsLeaveWorkEarly &&
                   EmployeeId == clocking.EmployeeId;
        }

        public void UpdateClockingPaymentStatus(byte clockingPaymentStatus)
        {
            ClockingPaymentStatus = clockingPaymentStatus;
        }

        #endregion

        #region private
        private void SetTimeByShift(DateTime startTime, Shift shift)
        {
            StartTime = startTime.Date.AddMinutes(shift.From);
            EndTime = startTime.Date.AddMinutes(shift.To);
            if (shift.From > shift.To)
                EndTime = EndTime.AddDays(1);
        }

        private bool HasLeaveOfAbsence(Clocking clocking)
        {
            // Trong TH nghỉ làm không phép
            if (clocking.AbsenceType == (byte)AbsenceTypes.UnauthorisedAbsence)
                // Nếu clocking trước có trạng thái nghỉ hoặc đã chấm công ra => xác nhận có sự thay đổi
                return AbsenceType == null && CheckOutDate == null;

            // Trong TH nghỉ làm có phép
            if (clocking.AbsenceType == (byte)AbsenceTypes.AuthorisedAbsence)
            {
                // Nếu clocking trước có trạng thái nghỉ làm không phép hoặc chưa chấm công ra
                // Hoặc nếu đã chấm công ra và có thời gian làm thêm, đi muộn, về sớm
                // => Xác nhận có sự thay đổi
                return AbsenceType != (byte)AbsenceTypes.UnauthorisedAbsence &&
                       CheckOutDate != null &&
                       (CheckOutDate == null || (OverTimeAfterShiftWork == 0 &&
                                                 OverTimeBeforeShiftWork == 0 &&
                                                 TimeIsLate == 0 && TimeIsLeaveWorkEarly == 0));
            }

            return false;
        }
        #endregion
    }
}