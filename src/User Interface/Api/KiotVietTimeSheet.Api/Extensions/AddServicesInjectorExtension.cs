using KiotVietTimeSheet.Application.Service.Impls;
using KiotVietTimeSheet.Application.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.Api.Extensions
{
    public static class AddServicesInjectorExtension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IPosParamService, PosParamService>();
            services.AddScoped<IImportExportService, ImportExportService>();
        }
    }
}
