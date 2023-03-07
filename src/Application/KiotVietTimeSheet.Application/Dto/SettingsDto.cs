namespace KiotVietTimeSheet.Application.Dto
{
    public class SettingsDto
    {
        public int EarlyTime { get; set; }
        public int LateTime { get; set; }
        public int EarlyTimeOT { get; set; }
        public int LateTimeOT { get; set; }
        public byte StartDateOfEveryMonth { get; set; }
        public byte FirstStartDateOfTwiceAMonth { get; set; }
        public byte SecondStartDateOfTwiceAMonth { get; set; }
        public byte StartDayOfWeekEveryWeek { get; set; }
        public byte StartDayOfWeekTwiceWeekly { get; set; }
        public int StandardWorkingDay { get; set; }

        #region ON/ OFF tính năng tự động tính Đi muộn, Về sớm, Làm thêm giờ
        public bool IsAutoCalcEarlyTime { get; set; }
        public bool IsAutoCalcEarlyTimeOT { get; set; }
        public bool IsAutoCalcLateTimeOT { get; set; }
        public bool IsAutoCalcLateTime { get; set; }
        #endregion

        #region Tự động ghi nhận giờ chấm công theo giờ bắt đầu, kết thúc ca
        public bool IsAutoTimekeepingMultiple { get; set; }
        public int MaxShiftIsAutoTimekeepingMultiple { get; set; }
        public int RangeShiftIsAutoTimekeepingMultipleHours { get; set; }
        public int RangeShiftIsAutoTimekeepingMultipleMinutes { get; set; }
        #endregion

        #region ON/OFF thông tin Giờ vào, Giờ ra, Đi muộn, Về sớm
        public bool IsDisplayCheckIn { get; set; }
        public bool IsDisplayCheckOut { get; set; }
        public bool IsDisplayLateTime { get; set; }
        public bool IsDisplayEarlyTime { get; set; }
        public bool IsDisplayEarlyTimeOT { get; set; }
        public bool IsDisplayLateTimeOT { get; set; }
        #endregion

        #region SWITCH giờ mặc định chấm công
        public int DefaultTimeSetting { get; set; }
        #endregion

        #region Tính nửa công

        public bool HalfShiftIsActive { get; set; }

        public int HalfShiftMaxHour { get; set; }

        public int HalfShiftMaxMinute { get; set; }

        #endregion
        #region Checkbox thiết lập tính lương và tự động tạo bảng lương

        public bool IsDateOfEveryMonth { get; set; }
        public bool IsDateOfTwiceAMonth { get; set; }
        public bool IsDayOfWeekEveryWeek { get; set; }
        public bool IsDayOfWeekTwiceWeekly { get; set; }
        public bool IsAutoCreatePaySheet { get; set; }

        #endregion

        #region Tự động chấm công theo ca làm việc

        public bool AllowAutoKeeping { get; set; }

        #endregion
        public bool IsImpactPayrateTemplate { get; set; }
        #region Thiết lập hoa hồng(value = 0 là lấy hết, value = 1 là chia đều)
        public int CommissionSetting { get; set; }
        #endregion
    }
}
