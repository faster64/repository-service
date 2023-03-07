using System;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.Common;

namespace KiotVietTimeSheet.Domain.Utilities
{
    public class Utilities
    {

        protected Utilities() { }
        /// <summary>
        /// Hàm kiểm tra một mốc thời gian có thuộc một khoảng thời gian xác định
        /// </summary>
        /// <param name="checkTime">Thời điểm đang xét</param>
        /// <param name="from">Thời điểm bắt đầu</param>
        /// <param name="to">Thời điểm kết thúc</param>
        /// <returns>true nếu thời điểm checkTime thuộc khoảng thời gian đang xét</returns>
        public static bool IsTimeInTimeRange(DateTime checkTime, DateTime from, DateTime to)
        {
            return from <= checkTime && to >= checkTime;
        }

        public static bool CompareDouble(double value, double compareValue)
        {
            return Math.Abs(value - compareValue) < KvTimeSheetConst.Tolerance;
        }

        /// <summary>
        /// Hàm xác định 2 chi tiết làm việc có được coi là overlap hay không
        /// </summary>
        /// <param name="firstClocking"></param>
        /// <param name="secondClocking"></param>
        /// <returns></returns>
        public static bool IsOverLapTimeBetweenClockings(Clocking firstClocking, Clocking secondClocking)
        {
            if (firstClocking.EmployeeId != secondClocking.EmployeeId)
            {
                return false;
            }
            if (firstClocking.ShiftId == secondClocking.ShiftId && firstClocking.EmployeeId == secondClocking.EmployeeId && firstClocking.StartTime.Date == secondClocking.StartTime.Date)
            {
                // 2 chi tiết làm việc đc coi là overlap nếu trùng ca, trùng nhân viên, trùng ngày bắt đầu làm việc
                return true;
            }
            if (firstClocking.StartTime < secondClocking.EndTime && firstClocking.EndTime > secondClocking.StartTime)
            {

                if (firstClocking.ShiftId == secondClocking.ShiftId)
                {
                    // xác định overlap nếu 2 chi tiết làm việc cùng ca
                    return true;
                }

                // xác định overlap khi 2 chi tiết làm việc khác ca và trùng lặp 1 khoảng tgian đc định trc (Constant.NumberOfHourOverLapBetween2Ranges) 
                var overlapHours = GetOverlappingHours(firstClocking.StartTime, firstClocking.EndTime, secondClocking.StartTime, secondClocking.EndTime);
                if (overlapHours > Constant.NumberOfHourOverLapBetween2Ranges)
                {
                    return true;
                }

            }

            return false;
        }

        /// <summary>
        /// Hàm xác định 2 ca làm việc có được coi là overlap hay không
        /// </summary>
        /// <param name="firstShift"></param>
        /// <param name="secondShift"></param>
        /// <returns></returns>
        public static bool IsOverLapTimeBetweenShifts(Shift firstShift, Shift secondShift)
        {

            var today = DateTime.Now.Date;
            var firstStart = today.AddMinutes(firstShift.From);
            var firstEnd = firstShift.From < firstShift.To ? today.AddMinutes(firstShift.To) : today.AddDays(1).AddMinutes(firstShift.To);

            var secondStart = today.AddMinutes(secondShift.From);
            var secondEnd = secondShift.From < secondShift.To ? today.AddMinutes(secondShift.To) : today.AddDays(1).AddMinutes(secondShift.To);

            if (firstStart < secondEnd && firstEnd > secondStart)
            {
                var overlapHours = GetOverlappingHours(firstStart, firstEnd, secondStart, secondEnd);
                if (overlapHours > Constant.NumberOfHourOverLapBetween2Ranges)
                {
                    return true;
                }

            }

            return false;
        }
        /// <summary>
        /// Xác định overlap giữa 2 khoẳng thời gian
        /// </summary>
        /// <param name="firstStart"></param>
        /// <param name="firstEnd"></param>
        /// <param name="secondStart"></param>
        /// <param name="secondEnd"></param>
        /// <param name="consideredOverlapWhenConsecutiveTime"> biến xác định có coi là overlap khi 2 khoảng thời gian là liên tiếp </param>
        /// <returns></returns>
        public static bool IsOverLapTimeBetweenRangeTimes(DateTime firstStart, DateTime firstEnd, DateTime secondStart, DateTime secondEnd, bool consideredOverlapWhenConsecutiveTime = false)
        {
            return consideredOverlapWhenConsecutiveTime ? firstStart <= secondEnd && firstEnd >= secondStart : firstStart < secondEnd && firstEnd > secondStart;
        }
        /// <summary>
        /// Tính khoảng thời gian overlap giữa 2 chi tiết làm việc
        /// </summary>
        /// <param name="firstStart"></param>
        /// <param name="firstEnd"></param>
        /// <param name="secondStart"></param>
        /// <param name="secondEnd"></param>
        /// <returns></returns>
        public static double GetOverlappingHours(DateTime firstStart, DateTime firstEnd, DateTime secondStart, DateTime secondEnd)
        {

            var maxStart = firstStart > secondStart ? firstStart : secondStart;
            var minEnd = firstEnd < secondEnd ? firstEnd : secondEnd;
            var interval = minEnd - maxStart;
            double returnValue = interval > TimeSpan.FromSeconds(0) ? interval.TotalHours : 0;
            return returnValue;
        }
        /// <summary>
        /// Chuyển đổi ngày sang giá trị text
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static string GetDayOfWeek(int day)
        {
            switch (day)
            {
                case (byte)DayOfWeek.Sunday:
                    return "Chủ nhật";
                case (byte)DayOfWeek.Monday:
                    return "Thứ hai";
                case (byte)DayOfWeek.Tuesday:
                    return "Thứ ba";
                case (byte)DayOfWeek.Wednesday:
                    return "Thứ tư";
                case (byte)DayOfWeek.Thursday:
                    return "Thứ năm";
                case (byte)DayOfWeek.Friday:
                    return "Thứ sáu";
                case (byte)DayOfWeek.Saturday:
                    return "Thứ bảy";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Tính giờ giữa 2 giá trị phút
        /// </summary>
        /// <param name="from">Phút ví dụ: 420p</param>
        /// <param name="to"> Phút ví dụ: 720p</param>
        /// <returns>720p - 420p: 300p/60 tức là 5h
        /// Trường hợp qua đêm: 1 ngày - thời gian bắt đầu + thời gian đến 
        /// </returns>
        /// 
        public static double GetHoursFromMinute(double? from, double? to)
        {
            if (from == null || to == null) return 0;
            var minutesForOneDay = 24 * 60;
            var getHours = (to - from) / 60;
            if (getHours >= 0) return (double)getHours;
            return (double)((minutesForOneDay - from + to) / 60);
        }

        public static bool CheckDateTimeIsOutTime(DateTime checkDateTime, DateTime endDateTime, DateTime startDateTime)
        {
            var standardMinutes = 300;
            if (checkDateTime >= endDateTime) return true;

            if ((endDateTime - startDateTime).TotalMinutes >= standardMinutes) return (endDateTime - checkDateTime).TotalMinutes <= 60;

            return (endDateTime - checkDateTime).TotalMinutes <= 20;
        }

        public static DateTimeFilter GetTimeRangeBase(DateTimeFilter time, bool fromCurrentDate)
        {
            switch (time.TimeRange.ToLower())
            {
                case "today":
                    time.StartDate = DateTime.Now.Date;
                    break;
                case "yesterday":
                    time.EndDate = DateTime.Now.Date;
                    time.StartDate = time.EndDate.Value.AddDays(-1);
                    break;
                case "2daybefore":
                    time.StartDate = DateTime.Now.Date.AddDays(-1);
                    break;
                case "3daybefore":
                    time.StartDate = DateTime.Now.Date.AddDays(-2);
                    break;
                case "thisweek":
                    time.StartDate = DateTimeHelper.FirstDateOfWeek(DateTime.Now.Date);
                    break;
                case "7day":
                case "7dayfordashboard":
                    time.StartDate = DateTime.Now.Date.AddDays(-6);
                    break;
                case "lastweek":
                    time.EndDate = DateTimeHelper.FirstDateOfWeek(DateTime.Now.Date);
                    time.StartDate = DateTimeHelper.FirstDateOfWeek(time.EndDate.Value.AddDays(-1));
                    break;
                case "month":
                    time.TimeRange = "M";
                    break;
                #region batch expire
                case "week_expire":
                    time.StartDate = DateTimeHelper.FirstDateOfWeek(DateTime.Now.Date);
                    time.EndDate = DateTimeHelper.FirstDateOfWeek(time.StartDate.Value.AddDays(7));
                    break;
                case "month_expire":
                    time.StartDate = DateTimeHelper.FirstDateOfMonth(DateTime.Now.Date);
                    time.EndDate = DateTimeHelper.FirstDateOfMonth(time.StartDate.Value.AddDays(31));
                    break;
                case "nextmonth_expire":
                    if (fromCurrentDate)
                    {
                        time.StartDate = DateTime.Now;
                        time.EndDate = time.StartDate.Value.AddMonths(1);
                    }
                    else
                    {
                        time.StartDate = DateTimeHelper.FirstDateOfMonth(DateTime.Now.Date.AddMonths(1));
                        time.EndDate = DateTimeHelper.FirstDateOfMonth(time.StartDate.Value.AddDays(31));
                    }
                    break;
                case "twomonths_expire":
                    if (fromCurrentDate)
                    {
                        time.StartDate = DateTime.Now;
                        time.EndDate = time.StartDate.Value.AddMonths(2);
                    }
                    else
                    {
                        time.StartDate = DateTimeHelper.FirstDateOfMonth(DateTime.Now.Date.AddMonths(1));
                        time.EndDate = DateTimeHelper.FirstDateOfMonth(DateTime.Now.Date.AddMonths(3));
                    }
                    break;
                case "threemonths_expire":
                    if (fromCurrentDate)
                    {
                        time.StartDate = DateTime.Now;
                        time.EndDate = time.StartDate.Value.AddMonths(3);
                    }
                    else
                    {
                        time.StartDate = DateTimeHelper.FirstDateOfMonth(DateTime.Now.Date.AddMonths(1));
                        time.EndDate = DateTimeHelper.FirstDateOfMonth(DateTime.Now.Date.AddMonths(4));
                    }
                    break;
                case "sixmonths_expire":
                    if (fromCurrentDate)
                    {
                        time.StartDate = DateTime.Now;
                        time.EndDate = time.StartDate.Value.AddMonths(6);
                    }
                    else
                    {
                        time.StartDate = DateTimeHelper.FirstDateOfMonth(DateTime.Now.Date.AddMonths(1));
                        time.EndDate = DateTimeHelper.FirstDateOfMonth(DateTime.Now.Date.AddMonths(7));
                    }
                    break;
                case "expire":
                    time.TimeRange = "E";
                    break;
                case "thewholetime_expire":
                    time.TimeRange = "W";
                    break;
                #endregion
                case "lastmonth":
                    time.EndDate = DateTimeHelper.FirstDateOfMonth(DateTime.Now.Date);
                    time.StartDate = DateTimeHelper.FirstDateOfMonth(time.EndDate.Value.AddDays(-1));
                    break;
                case "monthlunar":
                    time.StartDate = DateTimeHelper.FirstDateOfMonthLunar(DateTime.Now);
                    break;
                case "lastmonthlunar":
                    time.EndDate = DateTimeHelper.FirstDateOfMonthLunar(DateTime.Now);
                    time.StartDate = DateTimeHelper.FirstDateOfMonthLunar(time.EndDate.Value.AddDays(-1));
                    break;
                case "30day":
                    time.StartDate = DateTime.Now.Date.AddDays(-30);
                    break;
                case "quarter":
                    time.StartDate = DateTimeHelper.FirstDateOfQuarter(DateTime.Now.Date);
                    time.TimeRange = "Q";
                    break;
                case "lastquarter":
                    time.EndDate = DateTimeHelper.FirstDateOfQuarter(DateTime.Now.Date);
                    time.StartDate = DateTimeHelper.FirstDateOfQuarter(time.EndDate.Value.AddDays(-1));
                    break;
                case "year":
                    time.StartDate = DateTimeHelper.FirstDateOfYear(DateTime.Now.Date);
                    time.TimeRange = "Y";
                    break;
                case "lastyear":
                    time.EndDate = DateTimeHelper.FirstDateOfYear(DateTime.Now.Date);
                    time.StartDate = DateTimeHelper.FirstDateOfYear(time.EndDate.Value.AddDays(-1));
                    break;
                case "yearlunar":
                    time.StartDate = DateTimeHelper.FirstDateOfYearLunar(DateTime.Now);
                    break;
                case "lastyearlunar":
                    time.EndDate = DateTimeHelper.FirstDateOfYearLunar(DateTime.Now);
                    time.StartDate = DateTimeHelper.FirstDateOfYearLunar(time.EndDate.Value.AddDays(-1));
                    break;
                case "last2years":
                    time.EndDate = DateTime.Now.Date.AddDays(1);
                    time.StartDate = DateTimeHelper.FirstDateOfYear(DateTime.Now.Date.AddYears(-2));
                    break;
                case "last5years":
                    time.EndDate = DateTime.Now.Date.AddDays(1);
                    time.StartDate = DateTimeHelper.FirstDateOfYear(DateTime.Now.Date.AddYears(-5));
                    break;
                case "alltime":
                    time.EndDate = DateTime.Now;
                    break;
            }
            time.EndDate = time.EndDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
            return time;
        }

    }
}
