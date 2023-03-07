using KiotVietTimeSheet.Api.Configuration;
using KiotVietTimeSheet.Application.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.Api.IoC.NativeInjector
{
    public class NativeInjectorBootStrapper
    {
        protected NativeInjectorBootStrapper(){}
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IApiConfiguration>(c => new ApiConfiguration(configuration));
            services.AddSingleton<IApplicationConfiguration>(c => new ApplicationConfiguration(configuration));
            ApplicationLayerInjector.Register(services, configuration);
            DomainLayerInjector.Register(services, configuration);
            InfrastructureLayerInjector.Register(services, configuration);
        }
    }
}
