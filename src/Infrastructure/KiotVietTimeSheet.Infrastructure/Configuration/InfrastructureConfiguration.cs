using System.Collections.Generic;
using KiotViet.FileUpload.Providers.AmazoneS3;
using KiotVietTimeSheet.Infrastructure.Caching.Redis;
using KiotVietTimeSheet.Infrastructure.Configuration.ConfigurationModels;
using Microsoft.Extensions.Configuration;
using ServiceStack;

namespace KiotVietTimeSheet.Infrastructure.Configuration
{
    public  class InfrastructureConfiguration
    {
        public static string RequestHeaderRetailer => "Retailer";
        public static string RequestHeaderAuthorization => "Authorization";
        public static string RequestHeaderBranchId => "BranchId";
        public static string RequestHeaderKvVersion => "Kv-version";
        public static string RequestHeaderXGroupId => "X-GROUP-ID";
        public static string RequestHeaderXRetailerCode => "X-RETAILER-CODE";
        public static string AuthenticationSchemeType => "Bearer";
        public static List<RedisConfig> RedisConfigurations { get; } = new List<RedisConfig>();
        public static string ConnectionString { get; private set; }
        public static string ConnectionStringDbMaster { get; private set; }
        public static string GetBaseInformationApi { get; private set; }
        public static AmazoneS3FileUploadConfiguration AmazoneS3FileUploadConfiguration { get; } = new AmazoneS3FileUploadConfiguration();
        public static RedisConfig RedisConfiguration { get; } = new RedisConfig();
        public static RedisConfig RedisMqConfiguration { get; } = new RedisConfig();
        public static KiotVietServiceClientConfiguration KiotVietServiceClientConfiguration { get; } = new KiotVietServiceClientConfiguration();
        public static bool UseProfiler { get; private set; }
        public static string KvVersion { get; private set; }
        public InfrastructureConfiguration(IConfiguration configuration)
        {
            configuration.GetSection("redis.connections").Bind(RedisConfigurations);
            ConnectionString = configuration.GetConnectionString("KiotVietTimeSheetDatabase");
            ConnectionStringDbMaster = configuration.GetConnectionString("KiotVietMasterDatabase");
            GetBaseInformationApi = string.IsNullOrEmpty(configuration.GetConnectionString("GetBaseInformationApi")) ? "users/fnb-base-information/" : configuration.GetConnectionString("GetBaseInformationApi");
            configuration.GetSection("KiotVietFileUpload").Bind(AmazoneS3FileUploadConfiguration);
            configuration.GetSection("Caching:Redis").Bind(RedisConfiguration);
            configuration.GetSection("RedisMq").Bind(RedisMqConfiguration);
            configuration.GetSection("KiotVietApiClientConfig").Bind(KiotVietServiceClientConfiguration);
            UseProfiler = configuration.GetSection("UseProfiler").Get<bool>();
            KvVersion = configuration.GetSection("KiotVietApiClientConfig:KvVersion").Get<string>();
        }
    }
}
