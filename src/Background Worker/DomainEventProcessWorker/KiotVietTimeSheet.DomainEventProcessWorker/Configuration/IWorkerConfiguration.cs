using System.Collections.Generic;
using KiotVietTimeSheet.DomainEventProcessWorker.Configuration.ConfigModels;
using KiotVietTimeSheet.Infrastructure.Caching.Redis;

namespace KiotVietTimeSheet.DomainEventProcessWorker.Configuration
{
    public interface IWorkerConfiguration
    {
        RabbitMqEventBusConfiguration EventBusConfiguration { get; }
        string KvTimeSheetConnectionString { get; }
        string KvMasterConnectionString { get; }
        KiotVietServiceClientConfiguration KiotVietServiceClientConfiguration { get; }
        RedisConfig RedisConfiguration { get; }
        List<string> SendToWhenActiveTimeSheetEmails { get; }
        string SendFromWhenActiveTimeSheetEmail { get; }
        List<string> KiotMailServerList { get; }
        int KiotMailPort { get; }
        bool KiotMailUseSsl { get; }
        string KiotMailUsernameCertify { get; }
        string KiotMailPasswordCertify { get; }
        string KvCrmIntegrateEndpoint { get; }
        string KvCrmNote { get; }
    }
}
