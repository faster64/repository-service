using System;
using System.Threading.Tasks;
using KiotVietTimeSheet.Infrastructure.Persistence.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.Infrastructure.AuditTrail
{
    public interface IAuditProcessFailEventService
    {
        Task AddAsync(AuditProcessFailEvent @event);

        Task RetryAuditProcessFailEvents(Func<IntegrationEvent, Task> retryFunc);
    }
}
