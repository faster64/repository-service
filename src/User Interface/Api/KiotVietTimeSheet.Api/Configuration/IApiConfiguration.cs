using KiotVietTimeSheet.Api.Configuration.ConfigModels;

namespace KiotVietTimeSheet.Api.Configuration
{
    public interface IApiConfiguration
    {
        RabbitMqEventBusConfiguration EventBusConfiguration { get; }
        string AccessControlAllowOrigin { get; }
        string AccessControlAllowHeaders { get; }
        string AccessControlAllowHeadersValue { get; }
        string RootFolderName { get; }
        bool EnableSwaggerFeature { get; }
    }
}
