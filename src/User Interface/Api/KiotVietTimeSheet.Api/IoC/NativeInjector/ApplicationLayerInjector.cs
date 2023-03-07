using AutoMapper;
using KiotVietTimeSheet.Api.Extensions;
using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.Api.IoC.NativeInjector
{
    public class ApplicationLayerInjector
    {
        protected ApplicationLayerInjector(){}
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            // Components   
            services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService));

            // Validators
            services.AddValidators();

            // Domain Services
            services.AddDomainServices();

            // Event Handler Service
            services.AddEventHandlers();

            // Kiotvier api client service
            services.AddKiotVietApiClient();

            services.AddScoped<IAuthService, AuthService>();

            // Authenticate + Authorization + ExecutionContext
            services.AddExecutionContext();

            //Service
            services.AddServices();

            // Parameter
            services.AddParameterServices();

        }
    }
}
