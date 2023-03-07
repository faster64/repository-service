using System;

namespace KiotVietTimeSheet.Domain.Utilities
{
    public static class DateTimeHelper
    {
        public static DateTime FirstDateOfYearLunar(DateTime? dateTime = null)
        {
            var date = dateTime ?? DateTime.Now;
            var dateLunar = date.ToLunarDate();
            dateLunar.Day = 1;
            dateLunar.Month = 1;
            var sDate = dateLunar.ToSolarDate();
            return sDate;
        }

        public static DateTime FirstDateOfMonthLunar(DateTime? dateTime = null)
        {
            var date = dateTime ?? DateTime.Now;
            var dateLunar = date.ToLunarDate();
            dateLunar.Day = 1;
            return dateLunar.ToSolarDate();
        }

        public static DateTime LastDateOfMonthLunar(DateTime? dateTime = null)
        {
            var date = FirstDateOfMonthLunar(dateTime);
            return date.AddMonths(1).AddDays(-1);
        }

        public static int GetQuarter(DateTime datetime)
        {
            var quarterNumber = (datetime.Month - 1) / 3 + 1;
            return quarterNumber;
        }

        /// <summary>
        ///     Get first date of the quarter
        /// </summary>
        /// <param name="datetime">Date time input</param>
        /// <returns>First date of the month.</returns>
        public static DateTime FirstDateOfQuarter(DateTime? datetime = null)
        {
            var date = datetime ?? DateTime.Now;
            var quarterNumber = (date.Month - 1) / 3 + 1;
            var firstDayOfQuarter = new DateTime(date.Year, (quarterNumber - 1) * 3 + 1, 1);

            return firstDayOfQuarter;
        }

        public static DateTime FirstDateOfQuarter(int quarterNumber)
        {
            var firstDayOfQuarter = new DateTime(DateTime.Now.Year, (quarterNumber - 1) * 3 + 1, 1);
            return firstDayOfQuarter;
        }

        public static DateTime FirstDateOfQuarter(int year, int quarterNumber)
        {
            return new DateTime(year, (quarterNumber - 1) * 3 + 1, 1);
        }

        /// <summary>
        ///     Get last date of the quarter
        /// </summary>
        /// <param name="datetime">Date time input</param>
        /// <returns>First date of the month.</returns>
        public static DateTime LastDateOfQuarter(DateTime? datetime = null)
        {
            var date = datetime ?? DateTime.Now;
            var firstDayOfQuarter = FirstDateOfQuarter(date);
            var lastDayOfQuarter = firstDayOfQuarter.AddMonths(3).AddDays(-1);
            return lastDayOfQuarter;
        }

        /// <summary>
        ///     Get first date of the week
        /// </summary>
        /// <param name="datetime">Date time input</param>
        /// <returns>First date of the week.</returns>
        public static DateTime FirstDateOfWeek(DateTime datetime)
        {
            var day = datetime.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)datetime.DayOfWeek - 1;
            //   DayOfWeek.Friday
            return datetime.AddDays(-day);
        }

        /// <summary>
        ///     Get first date of the month
        /// </summary>
        /// <param name="datetime">Date time input</param>
        /// <returns>First date of the month.</returns>
        public static DateTime FirstDateOfMonth(DateTime datetime)
        {
            return new DateTime(datetime.Year, datetime.Month, 01);
        }

        /// <summary>
        ///     Get first date of year
        /// </summary>
        /// <param name="datetime">Date time input</param>
        /// <returns>First date of the year.</returns>
        public static DateTime FirstDateOfYear(DateTime datetime)
        {
            return new DateTime(datetime.Year, 01, 01);
        }

        /// <summary>
        ///     This function will find the last day of the month for the DateTime passed into it.
        /// </summary>
        /// <param name="dt">A DateTime of the month/year for which you need to find the last day.</param>
        /// <returns>A DateTime of the last day of the month for the month/year entered.</returns>
        public static DateTime LastDayofMonth(DateTime dt)
        {
            //Select the first day of the month by using the DateTime class
            dt = FirstDateOfMonth(dt);
            //Add one month to our adjusted DateTime
            dt = dt.AddMonths(1);
            //Subtract one day from our adjusted DateTime
            dt = dt.AddDays(-1);
            //Return the DateTime, now set to the last day of the month
            return dt;
        }

       
    }

    
}
