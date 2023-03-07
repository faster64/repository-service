using System;
using System.ComponentModel.DataAnnotations;

namespace KiotVietTimeSheet.Utilities
{
    public static class EnumHelpers
    {
        public static string GetDisplayValue(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo == null) return string.Empty;

            var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes == null) return string.Empty;

            var result = (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();

            return result;
        }
    }
}
