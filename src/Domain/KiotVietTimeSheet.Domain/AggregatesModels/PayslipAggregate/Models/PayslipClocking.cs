using KiotVietTimeSheet.SharedKernel.Models;
using Newtonsoft.Json;
using System;
using KiotVietTimeSheet.SharedKernel.Interfaces;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models
{
    public class PayslipClocking : BaseEntity,
        IEntityIdlong
    {
        #region Properties
        public long Id { get; set; }
        public long PayslipId { get; protected set; }
        public long ClockingId { get; protected set; }
        public DateTime? CheckInDate { get; protected set; }
        public DateTime? CheckOutDate { get; protected set; }
        /// <summary>
        /// Thời gian đi muộn
        /// </summary>
        public int TimeIsLate { get; set; }

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
        /// Trạng thái nghỉ <see cref="AbsenceType"/>
        /// null = k nghỉ
        /// </summary>
        public byte? AbsenceType { get; protected set; }

        /// <summary>
        /// Trạng thái chi tiết làm việc <see cref="ClockingStatus"/>
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
        /// Id của ca làm việc
        /// </summary>
        public long ShiftId { get; set; }
        #endregion

        #region Constructors
        public PayslipClocking() { }

        [JsonConstructor]
        public PayslipClocking(
            long payslipId,
            long clockingId,
            DateTime? checkInDate,
            DateTime? checkOutDate,
            int timeIsLate,
            int overTimeBeforeShiftWork,
            int timeIsLeaveWorkEarly,
            int overTimeAfterShiftWork,
            byte? absenceType,
            byte clockingStatus,
            DateTime startTime,
            DateTime endTime,
            long shiftId

        )
        {
            PayslipId = payslipId;
            ClockingId = clockingId;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            TimeIsLate = timeIsLate;
            OverTimeBeforeShiftWork = overTimeBeforeShiftWork;
            TimeIsLeaveWorkEarly = timeIsLeaveWorkEarly;
            OverTimeAfterShiftWork = overTimeAfterShiftWork;
            AbsenceType = absenceType;
            ClockingStatus = clockingStatus;
            StartTime = startTime;
            EndTime = endTime;
            ShiftId = shiftId;
        }
        #endregion

        #region Private methods
        #endregion
    }
}