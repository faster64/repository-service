using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.BackgroundTasks.Extensions
{
    public static class AddProcessExtension
    {
        public static void AddProcess(this IServiceCollection services)
        {
            services.AddScoped<CommissionDetailProcess>();
            services.AddScoped<PayslipPaymentProcess>();
            services.AddScoped<HolidayProcess>();
            services.AddScoped<ActiveTimesheetProcess>();
            services.AddScoped<SendMailProcess>();
            services.AddScoped<PaySheetProcess>();
            services.AddScoped<RealtimeProcess>();
            services.AddScoped<AuditLogProcess>();
            services.AddScoped<AutoGenerateClockingProcess>();
            services.AddScoped<UpdateClockingProcess>();

            services.AddScoped<ConfirmClockingProcess>();

            services.AddScoped<AutoKeepingProcess>();
        }
    }
}
