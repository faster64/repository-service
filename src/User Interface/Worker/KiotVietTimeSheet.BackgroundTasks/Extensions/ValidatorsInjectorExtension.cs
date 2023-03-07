using KiotVietTimeSheet.Application.Validators.EmployeeValidators;
using KiotVietTimeSheet.Application.Validators.HolidayValidators;
using KiotVietTimeSheet.Application.Validators.PayRateValidators;
using KiotVietTimeSheet.Application.Validators.SettingValidators;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.BackgroundTasks.Extensions
{
    public static class ValidatorsInjectorExtension
    {
        public static void AddValidators(this IServiceCollection services)
        {
            services.AddScoped<DepartmentCreateOrUpdateValidator>();
            services.AddScoped<JobTitleCreateOrUpdateValidator>();
            services.AddScoped<AllowanceCreateOrUpdateValidator>();
            services.AddScoped<DeductionCreateOrUpdateValidator>();
            services.AddScoped<CreateOrUpdateSettingValidator>();
            services.AddScoped<CreateOrUpdateHolidayAsyncValidator>();
        }
    }
}
