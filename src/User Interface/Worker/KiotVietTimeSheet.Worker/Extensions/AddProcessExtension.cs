using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.AuditTrailWorker.Extensions
{
    public static class AddProcessExtension
    {
        public static void AddProcess(this IServiceCollection services)
        {
            services.AddScoped<AutoTimeKeepingAuditProcess>();
            services.AddScoped<ClockingAuditProcess>();
            services.AddScoped<CommissionAuditProcess>();
            services.AddScoped<CommissionDetailAuditProcess>();
            services.AddScoped<EmployeeAuditProcess>();
            services.AddScoped<HolidayAuditProcess>();
            services.AddScoped<PayRateTemplateAuditProcess>();
            services.AddScoped<PaysheetAuditProcess>();
            services.AddScoped<PayslipAuditProcess>();
            services.AddScoped<SettingAuditProcess>();
            services.AddScoped<ShiftAuditProcess>();
            services.AddScoped<PenalizeAuditProcess>();
            services.AddScoped<GpsInfoAuditProcess>();
        }
    }
}
