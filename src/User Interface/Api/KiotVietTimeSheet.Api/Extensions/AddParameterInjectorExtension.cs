using KiotVietTimeSheet.Application.Parameters.Impls;
using KiotVietTimeSheet.Application.Parameters.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.Api.Extensions
{
    public static class AddParameterInjectorExtension
    {
        public static void AddParameterServices(this IServiceCollection services)
        {
            services.AddScoped<IUpdateConfirmClockingParamBuilder, UpdateConfirmClockingParamBuilder>();
        }
    }
}

