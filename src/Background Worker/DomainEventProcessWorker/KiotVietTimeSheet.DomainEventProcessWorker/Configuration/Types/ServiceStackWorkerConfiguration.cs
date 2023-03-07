using System.Collections.Generic;
using KiotVietTimeSheet.DomainEventProcessWorker.Configuration.ConfigModels;
using KiotVietTimeSheet.Infrastructure.Caching.Redis;
using ServiceStack.Configuration;

namespace KiotVietTimeSheet.DomainEventProcessWorker.Configuration.Types
{
    public class ServiceStackWorkerConfiguration : IWorkerConfiguration
    {
        private readonly IAppSettings _appSettings;
        public ServiceStackWorkerConfiguration(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }
        public RabbitMqEventBusConfiguration EventBusConfiguration =>
            _appSettings.Get<RabbitMqEventBusConfiguration>("EventBus:RabbitMq");

        public string KvTimeSheetConnectionString =>
            _appSettings.GetString("ConnectionStrings:KiotVietTimeSheetDatabase");

        public string KvMasterConnectionString =>
            _appSettings.GetString("ConnectionStrings:KiotVietMasterDatabase");

        public KiotVietServiceClientConfiguration KiotVietServiceClientConfiguration =>
            _appSettings.Get<KiotVietServiceClientConfiguration>("KiotVietApiClientConfig");

        public RedisConfig RedisConfiguration => _appSettings.Get<RedisConfig>("Caching:Redis");

        public List<string> SendToWhenActiveTimeSheetEmails => _appSettings.Get<List<string>>("SendToWhenActiveTimeSheetEmails");

        public string SendFromWhenActiveTimeSheetEmail => _appSettings.GetString("SendFromWhenActiveTimeSheetEmail");
        public List<string> KiotMailServerList => _appSettings.Get<List<string>>("KiotMailServerList");
        public int KiotMailPort => _appSettings.Get<int>("KiotMailPort");
        public bool KiotMailUseSsl => _appSettings.Get<bool>("KiotMailUseSsl");
        public string KiotMailUsernameCertify => _appSettings.GetString("KiotMailUsernameCertify");
        public string KiotMailPasswordCertify => _appSettings.GetString("KiotMailPasswordCertify");
        public string KvCrmIntegrateEndpoint => _appSettings.GetString("KvCrmIntegrateEndpoint");
        public string KvCrmNote => _appSettings.GetString("KvCrmNote");
    }
}
