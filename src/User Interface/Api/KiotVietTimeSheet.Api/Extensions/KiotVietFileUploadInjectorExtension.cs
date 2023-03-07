using KiotViet.FileUpload;
using KiotViet.FileUpload.Providers.AmazoneS3;
using KiotVietTimeSheet.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KiotVietTimeSheet.Api.Extensions
{
    public static class KiotVietFileUploadInjectorExtension
    {
        public static void AddKiotVietFileUpload(this IServiceCollection services)
        {
            services.AddScoped<IKiotVietFileUpload>(c => new AmazoneS3FileUploadProvider(InfrastructureConfiguration.AmazoneS3FileUploadConfiguration));
        }
    }
}
