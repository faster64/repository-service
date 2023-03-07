using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.DomainService.Impls;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.Api.Extensions
{
    public static class DomainServicesInjectorExtension
    {
        public static void AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IGenerateClockingsDomainService, GenerateClockingsDomainService>();
            services.AddScoped<IRejectClockingsDomainService, RejectClockingsDomainService>();
            services.AddScoped<IBatchUpdateTimeSheetClockingsDomainService, BatchUpdateTimeSheetClockingsDomainService>();
            services.AddScoped<ICreateTimeSheetClockingDomainService, CreateTimeSheetClockingDomainService>();
            services.AddScoped<IGetTimeSheetByBranchWorkingDaysDomainService, GetTimeSheetByBranchWorkingDaysDomainService>();
            services.AddScoped<IWorkingDayForPaysheetDomainService, WorkingDayForPaysheetDomainService>();
            services.AddScoped<IInitDraftPayslipsDomainService, InitDraftPayslipsDomainService>();
            services.AddScoped<ICreateOrUpdatePayslipDomainService, CreateOrUpdatePayslipDomainService>();
            services.AddScoped<ICreateOrUpdatePayRateDomainService, CreateOrUpdatePayRateDomainService>();
            services.AddScoped<IPaySheetOutOfDateDomainService, PaySheetOutOfDateDomainService>();
            services.AddScoped<ICancelPayslipDomainService, CancelPayslipDomainService>();
            services.AddScoped<IDetectionChangePaysheetDomainService, DetectionChangePaysheetDomainService>();
            services.AddScoped<IUpdateTotalPaymentPayslipDomainService, UpdateTotalPaymentPayslipDomainService>();
            services.AddScoped<ICompleteSalaryClockingDomainService, CompleteSalaryClockingDomainService>();
            services.AddScoped<IUpdateTenantNationalHolidayDomainService, UpdateTenantNationalHolidayDomainService>();
            services.AddScoped<IBatchCreateHolidayDomainService, BatchCreateHolidayDomainService>();
            services.AddScoped<IAutoTimeKeepingDomainService, AutoTimeKeepingDomainService>();
            services.AddScoped<IDeleteEmployeeDomainService, DeleteEmployeeDomainService>();
            services.AddScoped<IConfirmClockingDomainService, ConfirmClockingDomainService>();
            services.AddScoped<ICalculateTimeClockingDomainService, CalculateTimeClockingDomainService>();
        }
    }
}
