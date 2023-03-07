using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.Events;
using KiotVietTimeSheet.DomainEventProcessWorker.KvProcessFailEvent;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using Quartz;
using ServiceStack;

namespace KiotVietTimeSheet.DomainEventProcessWorker.ScheduleJobs
{
    [DisallowConcurrentExecution]
    public class RetryKvProcessFailEventJob : IJob
    {
        private ILogger<RetryKvProcessFailEventJob> _logger;
        private ILogger<RetryKvProcessFailEventJob> Logger => _logger ?? (_logger = HostContext.Resolve<ILogger<RetryKvProcessFailEventJob>>());

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var kvProcessFailEventService = HostContext.Resolve<IKvProcessFailEventService>();
                try
                {
                    await kvProcessFailEventService.RetryKvProcessFailEvents(async e =>
                    {
                        var eventType = Assembly.GetAssembly(typeof(RabbitPublishFailIntegrationEvent))
                            .GetTypes()
                            .FirstOrDefault(f => f.Name == e.GetType().Name);
                        if (eventType == null) return;
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        var handler = HostContext.Container.TryResolve(concreteType);
                        if (handler == null) return;
                        var method = concreteType.GetMethod("Handle")?.Invoke(handler, new object[] { e });
                        if (method == null) return;
                        await (Task)method;
                    });
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
        }
    }
}
