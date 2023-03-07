using KiotVietTimeSheet.Api.Extensions;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Infrastructure.DbRetail;
using KiotVietTimeSheet.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.Api.IoC.NativeInjector
{
    public class InfrastructureLayerInjector
    {
        protected InfrastructureLayerInjector(){}
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            // Read Repositories
            services.AddReadOnlyRepositories();

            // Write Repositories
            services.AddWriteOnlyRepositories();

            // KiotViet FileUpload
            services.AddKiotVietFileUpload();

            // Register Caching
            services.AddCaching();

            //register Internal service
            services.AddInternalService();

            services.AddScoped<IRetailDbService, RetailDbService>();
        }
    }
}
