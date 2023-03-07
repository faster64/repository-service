using System;
using System.Collections.Generic;
using System.Text;

namespace KiotVietTimeSheet.Utilities
{
    public static class PhoneNumberHelper
    {
        public static string StandardizePhoneNumber(string str, bool isReverse)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (c >= '0' && c <= '9')
                {
                    sb.Append(c);
                }
            }
            if (!string.IsNullOrEmpty(sb.ToString()) && sb.ToString().StartsWith("84"))
            {
                sb.Replace("84", "0");
            }
            return isReverse ? Reverse(sb.ToString()) : sb.ToString();
        }
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
