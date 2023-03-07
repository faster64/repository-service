using ServiceStack;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Utilities.Const
{
    public static class KvConst
    {
        public static Dictionary<string, Dictionary<string, string>> DayNameList { get; } = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "vi-VN",
                new Dictionary<string, string>
                {
                    {"0", "CN"},
                    {"1", "T2"},
                    {"2", "T3"},
                    {"3", "T4"},
                    {"4", "T5"},
                    {"5", "T6"},
                    {"6", "T7"},
                }
            },
            {
                "en-US",
                new Dictionary<string, string>
                {
                    {"0", "Su"},
                    {"1", "Mo"},
                    {"2", "Tu"},
                    {"3", "We"},
                    {"4", "Th"},
                    {"5", "Fr"},
                    {"6", "Sa"},
                }
            }
        };

        public static string GetDayNameList(string language, string daysOfWeek)
        {
            if (string.IsNullOrEmpty(daysOfWeek)) return string.Empty;
            var result = new List<string>();
            var days = daysOfWeek.Split(',');
            DayNameList.TryGetValue(language, out var dayByLang);
            if (dayByLang == null) return string.Empty;
            foreach (var day in days)
            {
                dayByLang.TryGetValue(day, out var dayName);
                if (!string.IsNullOrEmpty(dayName)) result.Add(dayName);
            }
            return result.Join(",");
        }
    }
}