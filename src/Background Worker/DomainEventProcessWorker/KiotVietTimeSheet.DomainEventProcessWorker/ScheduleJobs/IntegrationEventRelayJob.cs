using Quartz;
using System;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace KiotVietTimeSheet.DomainEventProcessWorker.ScheduleJobs
{
    [DisallowConcurrentExecution]
    public class IntegrationEventRelayJob : IJob
    {
        private ILogger<IntegrationEventRelayJob> _logger;
        private ILogger<IntegrationEventRelayJob> Logger => _logger ?? (_logger = HostContext.Resolve<ILogger<IntegrationEventRelayJob>>());

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var integrationEventLogEntryRepository = HostContext.Resolve<ITimeSheetIntegrationEventService>();
                try
                {
                    await integrationEventLogEntryRepository.RetryPublishFailedEventToEventBusAsync();
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
