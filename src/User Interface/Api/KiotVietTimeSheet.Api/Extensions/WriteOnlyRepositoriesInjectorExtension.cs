using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.Api.Extensions
{
    public static class WriteOnlyRepositoriesInjectorExtension
    {
        public static void AddWriteOnlyRepositories(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeWriteOnlyRepository, EmployeeWriteOnlyRepository>();
            services.AddScoped<IEmployeeProfilePictureWriteOnlyRepository, EmployeeProfilePictureWriteOnlyRepository>();
            services.AddScoped<IDepartmentWriteOnlyRepository, DepartmentWriteOnlyRepository>();
            services.AddScoped<IJobTitleWriteOnlyRepository, JobTitleWriteOnlyRepository>();

            services.AddScoped<IPayRateWriteOnlyRepository, PayRateWriteOnlyRepository>();
            services.AddScoped<IPayRateDetailWriteOnlyRepository, PayRateDetailWriteOnlyRepository>();
            services.AddScoped<IPayRateTemplateWriteOnlyRepository, PayRateTemplateWriteOnlyRepository>();
            services.AddScoped<IPayRateTemplateDetailWriteOnlyRepository, PayRateTemplateDetailWriteOnlyRepository>();
            services.AddScoped<IAllowanceWriteOnlyRepository, AllowanceWriteOnlyRepository>();
            services.AddScoped<IDeductionWriteOnlyRepository, DeductionWriteOnlyRepository>();

            services.AddScoped<ITimeSheetWriteOnlyRepository, TimeSheetWriteOnlyRepository>();
            services.AddScoped<ITimeSheetShiftWriteOnlyRepository, TimeSheetShiftWriteOnlyRepository>();
            services.AddScoped<IClockingWriteOnlyRepository, ClockingWriteOnlyRepository>();
            services.AddScoped<IClockingHistoryWriteOnlyRepository, ClockingHistoryWriteOnlyRepository>();
            services.AddScoped<IShiftWriteOnlyRepository, ShiftWriteOnlyRepository>();
            services.AddScoped<IHolidayWriteOnlyRepository, HolidayWriteOnlyRepository>();
            services.AddScoped<ITenantNationalHolidayWriteOnlyRepository, TenantNationalHolidayWriteOnlyRepository>();
            services.AddScoped<IPenalizeWriteOnlyRepository, PenalizeWriteOnlyRepository>();
            services.AddScoped<IClockingPenalizeWriteOnlyRepository, ClockingPenalizeWriteOnlyRepository>();

            services.AddScoped<ISettingsWriteOnlyRepository, SettingsWriteOnlyRepository>();

            services.AddScoped<IBranchSettingWriteOnlyRepository, BranchSettingWriteOnlyRepository>();

            services.AddScoped<IPaysheetWriteOnlyRepository, PaysheetWriteOnlyRepository>();
            services.AddScoped<IPayslipWriteOnlyRepository, PayslipWriteOnlyRepository>();
            services.AddScoped<IPayslipDetailWriteOnlyRepository, PayslipDetailWriteOnlyRepository>();
            services.AddScoped<IPayslipClockingWriteOnlyRepository, PayslipClockingWriteOnlyRepository>();
            services.AddScoped<IPayslipClockingPenalizeWriteOnlyRepository, PayslipClockingPenalizeWriteOnlyRepository>();
            services.AddScoped<IPayslipPenalizeWriteOnlyRepository, PayslipPenalizeWriteOnlyRepository>();

            services.AddScoped<IFingerMachineWriteOnlyRepository, FingerMachineWriteOnlyRepository>();
            services.AddScoped<IFingerPrintWriteOnlyRepository, FingerPrintWriteOnlyRepository>();

            services.AddScoped<ICommissionWriteOnlyRepository, CommissionWriteOnlyRepository>();
            services.AddScoped<ICommissionDetailWriteOnlyRepository, CommissionDetailWriteOnlyRepository>();
            services.AddScoped<ICommissionBranchWriteOnlyRepository, CommissionBranchWriteOnlyRepository>();
            services.AddScoped<IEmployeeBranchWriteOnlyRepository, EmployeeBranchWriteOnlyRepository>();

            services.AddScoped<IGpsInfoWriteOnlyRepository, GpsInfoWriteOnlyRepository>();
            services.AddScoped<IConfirmClockingWriteOnlyRepository, ConfirmClockingWriteOnlyRepository>();
            services.AddScoped<IConfirmClockingHistoryWriteOnlyRepository, ConfirmClockingHistoryWriteOnlyRepository>();
        }
    }
}
