using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Infrastructure.Caching.Repositories;
using KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.AuditTrailWorker.Extensions
{
    public static class AddResponsitoriesExtension
    {
        public static void AddResponsitories(this IServiceCollection services)
        {
            services.AddScoped<IPayRateReadOnlyRepository, PayRateReadOnlyRepository>();
            services.AddScoped<IShiftReadOnlyRepository, CachedShiftReadOnlyRepository>();
            services.AddScoped<IDepartmentReadOnlyRepository, CachedDepartmentReadOnlyRepository>();
            services.AddScoped<IJobTitleReadOnlyRepository, CachedJobTitleReadOnlyRepository>();
            services.AddScoped<IAllowanceReadOnlyRepository, AllowanceReadOnlyRepository>();
            services.AddScoped<IDeductionReadOnlyRepository, DeductionReadOnlyRepository>();
            services.AddScoped<ICommissionReadOnlyRepository, CommissionReadOnlyRepository>();
            services.AddScoped<IEmployeeReadOnlyRepository, EmployeeReadOnlyRepository>();
            services.AddScoped<IPaysheetReadOnlyRepository, PaysheetReadOnlyRepository>();
            services.AddScoped<IPayslipReadOnlyRepository, PayslipReadOnlyRepository>();
            services.AddScoped<IFingerPrintReadOnlyRepository, FingerPrintReadOnlyRepository>();
            services.AddScoped<IFingerMachineReadOnlyRepository, FingerMachineReadOnlyRepository>();
            services.AddScoped<IPenalizeReadOnlyRepository, PenalizeReadOnlyRepository>();
        }
    }
}
