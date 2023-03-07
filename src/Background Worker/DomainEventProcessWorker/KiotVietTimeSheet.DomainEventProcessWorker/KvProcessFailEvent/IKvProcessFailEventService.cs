using System;
using System.Threading.Tasks;
using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.DomainEventProcessWorker.KvProcessFailEvent
{
    public interface IKvProcessFailEventService
    {
        Task RetryKvProcessFailEvents(Func<IntegrationEvent, Task> retryFunc);
    }
}
