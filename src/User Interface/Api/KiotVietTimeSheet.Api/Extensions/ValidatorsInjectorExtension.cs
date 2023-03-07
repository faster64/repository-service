using KiotVietTimeSheet.Application.Validators.EmployeeValidators;
using KiotVietTimeSheet.Application.Validators.GpsInfoValidators;
using KiotVietTimeSheet.Application.Validators.HolidayValidators;
using KiotVietTimeSheet.Application.Validators.PayRateValidators;
using KiotVietTimeSheet.Application.Validators.SettingValidators;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.Api.Extensions
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
            services.AddScoped<PenalizeCreateOrUpdateValidator>();
            services.AddScoped<GpsInfoCreateOrUpdateValidator>();
        }
    }
}
