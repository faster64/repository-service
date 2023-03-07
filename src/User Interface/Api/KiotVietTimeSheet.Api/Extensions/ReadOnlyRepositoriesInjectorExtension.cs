using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Infrastructure.Caching.Repositories;
using KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.Api.Extensions
{
    public static class ReadOnlyRepositoriesInjectorExtension
    {
        public static void AddReadOnlyRepositories(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeReadOnlyRepository, CachedEmployeeReadOnlyRepository>();
            services.AddScoped<IEmployeeProfilePictureReadOnlyRepository, EmployeeProfilePictureReadOnlyRepository>();
            services.AddScoped<IDepartmentReadOnlyRepository, CachedDepartmentReadOnlyRepository>();
            services.AddScoped<IJobTitleReadOnlyRepository, CachedJobTitleReadOnlyRepository>();

            services.AddScoped<IPayRateReadOnlyRepository, PayRateReadOnlyRepository>();
            services.AddScoped<IPayRateDetailReadOnlyRepository, PayRateDetailReadOnlyRepository>();
            services.AddScoped<IPayRateTemplateReadOnlyRepository, PayRateTemplateReadOnlyRepository>();
            services.AddScoped<IPayRateTemplateDetailReadOnlyRepository, PayRateTemplateDetailReadOnlyRepository>();
            services.AddScoped<IAllowanceReadOnlyRepository, AllowanceReadOnlyRepository>();
            services.AddScoped<IDeductionReadOnlyRepository, DeductionReadOnlyRepository>();

            services.AddScoped<ITimeSheetReadOnlyRepository, TimeSheetReadOnlyRepository>();
            services.AddScoped<IClockingReadOnlyRepository, ClockingReadOnlyRepository>();
            services.AddScoped<IClockingHistoryReadOnlyRepository, ClockingHistoryReadOnlyRepository>();
            services.AddScoped<IShiftReadOnlyRepository, CachedShiftReadOnlyRepository>();
            services.AddScoped<IHolidayReadOnlyRepository, CachedHolidayReadOnlyRepository>();
            services.AddScoped<INationalHolidayReadOnlyRepository, NationalHolidayReadOnlyRepository>();
            services.AddScoped<ITenantNationalHolidayReadOnlyRepository, TenantNationalHolidayReadOnlyRepository>();
            services.AddScoped<IPenalizeReadOnlyRepository, PenalizeReadOnlyRepository>();

            services.AddScoped<ISettingsReadOnlyRepository, SettingsReadOnlyRepository>();

            services.AddScoped<IBranchSettingReadOnlyRepository, BranchSettingReadOnlyRepository>();

            services.AddScoped<IPaysheetReadOnlyRepository, PaysheetReadOnlyRepository>();
            services.AddScoped<IPayslipReadOnlyRepository, PayslipReadOnlyRepository>();
            services.AddScoped<IPayslipDetailReadOnlyRepository, PayslipDetailReadOnlyRepository>();
            services.AddScoped<IPayslipClockingReadOnlyRepository, PayslipClockingReadOnlyRepository>();
            services.AddScoped<IPayslipClockingPenalizeReadOnlyRepository, PayslipClockingPenalizeReadOnlyRepository>();

            services.AddScoped<IFingerMachineReadOnlyRepository, FingerMachineReadOnlyRepository>();
            services.AddScoped<IFingerPrintReadOnlyRepository, FingerPrintReadOnlyRepository>();

            services.AddScoped<ICommissionReadOnlyRepository, CommissionReadOnlyRepository>();
            services.AddScoped<ICommissionDetailReadOnlyRepository, CommissionDetailReadOnlyRepository>();
            services.AddScoped<ICommissionBranchReadOnlyRepository, CommissionBranchReadOnlyRepository>();
            services.AddScoped<IEmployeeBranchReadOnlyRepository, EmployeeBranchReadOnlyRepository>();
            services.AddScoped<IPayslipPenalizeReadOnlyRepository, PayslipPenalizeReadOnlyRepository>();

            services.AddScoped<IGpsInfoReadOnlyRepository, CachedGpsInfoReadOnlyRepository>();
            services.AddScoped<IConfirmClockingReadOnlyRepository, ConfirmClockingReadOnlyRepository>();
            services.AddScoped<IConfirmClockingHistoryReadOnlyRepository, ConfirmClockingHistoryReadOnlyRepository>();

            services.AddScoped<ITrialDataReadOnlyRepository, TrialDataReadOnlyRepository>();
        }
    }
}
