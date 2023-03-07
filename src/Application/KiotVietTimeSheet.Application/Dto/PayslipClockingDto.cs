using System;

namespace KiotVietTimeSheet.Application.Dto
{
    public class PayslipClockingDto
    {
        public long Id { get; set; }
        public long PayslipId { get; protected set; }
        public long ClockingId { get; protected set; }
        public DateTime? CheckInDate { get; protected set; }
        public DateTime? CheckOutDate { get; protected set; }
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
    }
}
