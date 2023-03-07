using System;
using System.Globalization;
using System.Linq;
using Google.Authenticator;
using Microsoft.AspNetCore.Http;

namespace KiotVietTimeSheet.Utilities
{
    public static class Globals
    {
        #region constraint
        public const int EmployeeConfirmPinInSeconds = 5 * 60;
        #endregion

        public static string BuildVersion { get; private set; }

        public static void SetBuildVersion()
        {
            var dockerImageBuildArg = Environment.GetEnvironmentVariable("DOCKER_IMAGE_BUILD_ARG");
            BuildVersion = string.IsNullOrEmpty(dockerImageBuildArg) ? "Build version not set" : dockerImageBuildArg;
        }

        public static string GetRetailerCode(IHttpContextAccessor httpContext)
        {
            var context = httpContext.HttpContext;
            if (context == null || context.Request == null) return string.Empty;
            var retailerCode = context.Request.Headers["Retailer"].ToString() ?? string.Empty;

            // Check it from cookie
            if (string.IsNullOrEmpty(retailerCode))
            {
                retailerCode = context.Request.Cookies["Retailer"] ?? string.Empty;
            }

            return retailerCode.ToLower(CultureInfo.CurrentCulture).Trim();
        }
        public static object GetPropValue(object src, string propName)
        {
            if (src == null) return null;
            var prop = src.GetType().GetProperty(propName);
            return prop == null ? null : prop.GetValue(src, null);
        }

        public static string GetcacheKey(this string key, string type)
        {
            return $"cache:{type}:{key}";
        }

        public static string GetCacheAutoGenerateClockingInprogress(int tenantId)
        {
            return $"cache:inprogress:AutoGenerateClocking:{tenantId}";
        }

        public static AutoGenerateClockingStatus GetAutoGenerateClockingStatus(bool isRepeat, bool hasEndDate)
        {
            return (!isRepeat || hasEndDate) ? AutoGenerateClockingStatus.NoAuto : AutoGenerateClockingStatus.Auto;
        }

        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GetTwoFaPin(string secretKey, int timeToleranceInSeconds)
        {
            var tfa = new TwoFactorAuthenticator();
            var timeTolerance = TimeSpan.FromSeconds(timeToleranceInSeconds);
            var codeIndex = (int)Math.Ceiling((decimal)(timeToleranceInSeconds / 30));
            var codes = tfa.GetCurrentPINs(secretKey, timeTolerance);

            return codes[codeIndex];
        }

        public static bool ValidateTwoFactorPin(string secretKey, int timeToleranceInSeconds, string pin)
        {
            var tfa = new TwoFactorAuthenticator();
            var timeTolerance = TimeSpan.FromSeconds(timeToleranceInSeconds);
            var result = tfa.ValidateTwoFactorPIN(secretKey, pin, timeTolerance);

            return result;
        }

        public static double GetDistance(double lat1, double long1, double lat2, double long2)
        {
            var r = 6378137;
            var dLat = Rad(lat2 - lat1);
            var dLong = Rad(long2 - long1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(Rad(lat1)) * Math.Cos(Rad(lat2)) *
              Math.Sin(dLong / 2) * Math.Sin(dLong / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = r * c;
            return d;

            double Rad(double x)
            {
                return x * Math.PI / 180;
            }
        }
    }
}
