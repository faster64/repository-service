using System.Collections.Generic;
using System.Reflection;

namespace KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models
{
    public class SettingsToObject
    {

        public SettingsToObject()
        {

        }
        public Dictionary<string, string> Data { get; set; }
        public int EarlyTime
        {
            get
            {
                int.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, 0), out var intParse);
                return intParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public int EarlyTimeOT
        {
            get
            {
                int.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, 0), out var intParse);
                return intParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public int LateTime
        {
            get
            {
                int.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, 0), out var intParse);
                return intParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public int LateTimeOT
        {
            get
            {
                int.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, 0), out var intParse);
                return intParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public byte StartDateOfEveryMonth
        {
            get
            {
                byte.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, 1), out var intParse);
                return intParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public byte FirstStartDateOfTwiceAMonth
        {
            get
            {
                byte.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, 1), out var intParse);
                return intParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public byte SecondStartDateOfTwiceAMonth
        {
            get
            {
                byte.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, 16), out var intParse);
                return intParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public byte StartDayOfWeekEveryWeek
        {
            get
            {
                byte.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, 1), out var byteParse);
                return byteParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public byte StartDayOfWeekTwiceWeekly
        {
            get
            {
                byte.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, 1), out var byteParse);
                return byteParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public int StandardWorkingDay
        {
            get
            {
                int.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, 8), out var intParse);
                return intParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        #region ON/ OFF tính năng tự động tính Đi muộn, Về sớm, Làm thêm giờ

        public bool IsAutoCalcEarlyTime
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, true), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public bool IsAutoCalcEarlyTimeOT
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, true), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public bool IsAutoCalcLateTimeOT
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, true), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public bool IsAutoCalcLateTime
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, true), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }
        #endregion

        #region  ON/OFF thông tin Giờ vào, Giờ ra, Đi muộn, Về sớm
        public bool IsDisplayCheckIn
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, true), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public bool IsDisplayCheckOut
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, true), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public bool IsDisplayLateTime
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, true), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public bool IsDisplayEarlyTime
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, true), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public bool IsDisplayEarlyTimeOT
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, true), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public bool IsDisplayLateTimeOT
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, true), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        #endregion

        #region Tự động ghi nhận giờ chấm công theo giờ bắt đầu, kết thúc ca

        public bool IsAutoTimekeepingMultiple
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, false), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public int MaxShiftIsAutoTimekeepingMultiple
        {
            get
            {
                int.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, 2), out var intParse);
                return intParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public int RangeShiftIsAutoTimekeepingMultipleHours
        {
            get
            {
                int.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, 1), out var intParse);
                return intParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }
        public int RangeShiftIsAutoTimekeepingMultipleMinutes
        {
            get
            {
                int.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, 0), out var intParse);
                return intParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        #endregion

        public string GetPropValue(string propName, object def)
        {
            propName = propName.Replace("get_", "").Replace("set_", "");
            var val = def.ToString();
            if (Data != null && Data.ContainsKey(propName))
            {
                val = Data[propName];
            }
            return val;
        }
        public void SetPropValue(string propName, object value)
        {
            propName = propName.Replace("get_", "").Replace("set_", "");
            if (Data.ContainsKey(propName))
            {
                Data[propName] = value.ToString();
            }
            else
            {
                Data.Add(propName, value.ToString());
            }

        }

        public int DefaultTimeSetting
        {
            get
            {
                int.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, 0), out var intParse);
                return intParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        #region Tính nửa công

        public bool HalfShiftIsActive
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, false), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public int HalfShiftMaxHour
        {
            get
            {
                int.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, 4), out var intParse);
                return intParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public int HalfShiftMaxMinute
        {
            get
            {
                int.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, 30), out var intParse);
                return intParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        #endregion

        public bool UseClockingGps
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, false), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }


        #region Checkbox thiết lập tính lương và tự động tạo bảng lương

        public bool IsDateOfEveryMonth
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, true), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }
        public bool IsDateOfTwiceAMonth
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, false), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public bool IsDayOfWeekEveryWeek
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, false), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }
        public bool IsDayOfWeekTwiceWeekly
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, false), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }
        public bool IsAutoCreatePaySheet
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, false), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        #endregion

        public bool AllowAutoKeeping
        {
            get
            {
                bool.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, false), out var boolParse);
                return boolParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

        public int CommissionSetting
        {
            get
            {
                int.TryParse(GetPropValue(MethodBase.GetCurrentMethod().Name, 0), out var intParse);
                return intParse;
            }
            set => SetPropValue(MethodBase.GetCurrentMethod().Name, value);
        }

    }
}
