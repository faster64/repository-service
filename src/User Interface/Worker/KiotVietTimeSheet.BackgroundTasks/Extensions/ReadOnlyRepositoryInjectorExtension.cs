using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.BackgroundTasks.Extensions
{
    public static class ReadOnlyRepositoryInjectorExtension
    {
        public static void AddReadOnlyRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICommissionReadOnlyRepository, CommissionReadOnlyRepository>();
            services.AddScoped<ICommissionDetailReadOnlyRepository, CommissionDetailReadOnlyRepository>();
            services.AddScoped<ICommissionBranchReadOnlyRepository, CommissionBranchReadOnlyRepository>();
            services.AddScoped<IEmployeeBranchReadOnlyRepository, EmployeeBranchReadOnlyRepository>();
            services.AddScoped<IPayslipReadOnlyRepository, PayslipReadOnlyRepository>();
            services.AddScoped<IPaysheetReadOnlyRepository, PaysheetReadOnlyRepository>();
            services.AddScoped<IPayslipDetailReadOnlyRepository, PayslipDetailReadOnlyRepository>();
            services.AddScoped<IEmployeeReadOnlyRepository, EmployeeReadOnlyRepository>();

            services.AddScoped<ITenantNationalHolidayReadOnlyRepository, TenantNationalHolidayReadOnlyRepository>();
            services.AddScoped<INationalHolidayReadOnlyRepository, NationalHolidayReadOnlyRepository>();
            services.AddScoped<IHolidayReadOnlyRepository, HolidayReadOnlyRepository>();

            services.AddScoped<IBranchSettingReadOnlyRepository, BranchSettingReadOnlyRepository>();

            services.AddScoped<IClockingReadOnlyRepository, ClockingReadOnlyRepository>();
            services.AddScoped<IPayRateReadOnlyRepository, PayRateReadOnlyRepository>();
            services.AddScoped<IDeductionReadOnlyRepository, DeductionReadOnlyRepository>();
            services.AddScoped<IAllowanceReadOnlyRepository, AllowanceReadOnlyRepository>();
            services.AddScoped<ISettingsReadOnlyRepository, SettingsReadOnlyRepository>();
            services.AddScoped<IShiftReadOnlyRepository, ShiftReadOnlyRepository>();
            services.AddScoped<IPenalizeReadOnlyRepository, PenalizeReadOnlyRepository>();

            services.AddScoped<IConfirmClockingReadOnlyRepository, ConfirmClockingReadOnlyRepository>();
        }
    }
}
