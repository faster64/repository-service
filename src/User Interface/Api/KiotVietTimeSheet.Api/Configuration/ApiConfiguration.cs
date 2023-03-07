using KiotVietTimeSheet.Api.Configuration.ConfigModels;
using Microsoft.Extensions.Configuration;

namespace KiotVietTimeSheet.Api.Configuration
{
    public class ApiConfiguration : IApiConfiguration
    {
        private readonly IConfiguration _configuration;

        public string AccessControlAllowOrigin => "Access-Control-Allow-Origin";
        public string AccessControlAllowHeaders => "Access-Control-Allow-Headers";
        public string AccessControlAllowHeadersValue => "Origin, X-Requested-With, Content-Type, Accept";
        public string RootFolderName => "wwwroot";
        public RabbitMqEventBusConfiguration EventBusConfiguration => _configuration.GetSection("EventBus:RabbitMq").Get<RabbitMqEventBusConfiguration>();
        public bool EnableSwaggerFeature => _configuration.GetSection("EnableSwaggerFeature").Get<bool>();

        public ApiConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
