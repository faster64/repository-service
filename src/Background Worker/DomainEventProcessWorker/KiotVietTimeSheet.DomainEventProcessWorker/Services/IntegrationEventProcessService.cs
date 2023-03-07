using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace KiotVietTimeSheet.DomainEventProcessWorker.Services
{
    public class IntegrationEventProcessService : BackgroundService
    {
        public IntegrationEventProcessService()
        {
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
