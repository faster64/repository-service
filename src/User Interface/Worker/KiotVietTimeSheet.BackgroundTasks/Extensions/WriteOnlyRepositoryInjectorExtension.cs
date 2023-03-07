using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.BackgroundTasks.Extensions
{
    public static class WriteOnlyRepositoryInjectorExtension
    {
        public static void AddWriteOnlyRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICommissionWriteOnlyRepository, CommissionWriteOnlyRepository>();
            services.AddScoped<ICommissionDetailWriteOnlyRepository, CommissionDetailWriteOnlyRepository>();
            services.AddScoped<ICommissionBranchWriteOnlyRepository, CommissionBranchWriteOnlyRepository>();
            services.AddScoped<IEmployeeBranchWriteOnlyRepository, EmployeeBranchWriteOnlyRepository>();
            services.AddScoped<IPaysheetWriteOnlyRepository, PaysheetWriteOnlyRepository>();
            services.AddScoped<IPayslipWriteOnlyRepository, PayslipWriteOnlyRepository>();
            services.AddScoped<IPayslipDetailWriteOnlyRepository, PayslipDetailWriteOnlyRepository>();
            services.AddScoped<IPayslipClockingWriteOnlyRepository, PayslipClockingWriteOnlyRepository>();
            services.AddScoped<IPayslipClockingPenalizeWriteOnlyRepository, PayslipClockingPenalizeWriteOnlyRepository>();
            services.AddScoped<IClockingWriteOnlyRepository, ClockingWriteOnlyRepository>();
            services.AddScoped<IPayslipPenalizeWriteOnlyRepository, PayslipPenalizeWriteOnlyRepository>();
            services.AddScoped<ITimeSheetWriteOnlyRepository, TimeSheetWriteOnlyRepository>();
            services.AddScoped<ITimeSheetShiftWriteOnlyRepository, TimeSheetShiftWriteOnlyRepository>();
            services.AddScoped<IAutoKeepingWriteOnlyRepository, AutoKeepingWriteOnlyRepository>();
            services.AddScoped<ITenantNationalHolidayWriteOnlyRepository, TenantNationalHolidayWriteOnlyRepository>();
            services.AddScoped<IHolidayWriteOnlyRepository, HolidayWriteOnlyRepository>();
        }
    }
}
