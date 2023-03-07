using System.Threading.Tasks;
using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.Infrastructure.EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }
}
