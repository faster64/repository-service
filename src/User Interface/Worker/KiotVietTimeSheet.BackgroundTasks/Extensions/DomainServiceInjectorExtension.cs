using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.DomainService.Impls;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.BackgroundTasks.Extensions
{
    public static class DomainServiceInjectorExtension 
    {
        public static void AddDomainService(this IServiceCollection services)
        {
            services.AddScoped<IPaySheetOutOfDateDomainService, PaySheetOutOfDateDomainService>();

            services.AddScoped<ICreateOrUpdatePayslipDomainService, CreateOrUpdatePayslipDomainService>();

            services.AddScoped<IGenerateClockingsDomainService, GenerateClockingsDomainService>();
            services.AddScoped<IBatchUpdateTimeSheetClockingsDomainService, BatchUpdateTimeSheetClockingsDomainService>();
        }
    }
}
