using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.ServiceClients;
using System.Globalization;
using System.Threading;

namespace KiotVietTimeSheet.BackgroundTasks.BackgroundProcess
{
    public class BaseBackgroundProcess
    {
        protected readonly IKiotVietInternalService _kiotVietInternalService;
        protected readonly IAuthService _authService;

        public BaseBackgroundProcess(IKiotVietInternalService kiotVietInternalService, IAuthService authService)
        {
            _kiotVietInternalService = kiotVietInternalService;
            _authService = authService;
            SetCultureLangue(authService.Context);
        }

        private static void SetCultureLangue(SharedKernel.Auth.ExecutionContext context)
        {
            var info = new CultureInfo(context?.Language ?? "vi-VN")
            {
                DateTimeFormat = { ShortDatePattern = "dd/MM/yyyy" },
                NumberFormat =
                {
                    CurrencyDecimalSeparator = ".",
                    CurrencyGroupSeparator = ",",
                    CurrencySymbol = string.Empty,
                    NumberDecimalSeparator = ".",
                    NumberGroupSeparator = ",",
                    PercentDecimalSeparator = ".",
                    PercentGroupSeparator = ","
                }
            };
            Thread.CurrentThread.CurrentCulture = info;
            Thread.CurrentThread.CurrentUICulture = info;
        }
    }
}