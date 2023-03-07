using MediatR;

namespace KiotVietTimeSheet.SharedKernel.Domain
{
    public interface IEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
        where TDomainEvent : DomainEvent
    {

    }
}
