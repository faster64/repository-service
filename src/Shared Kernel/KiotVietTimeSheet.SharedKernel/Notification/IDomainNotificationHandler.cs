using MediatR;

namespace KiotVietTimeSheet.SharedKernel.Notification
{
    public interface IDomainNotificationHandler<in TDomainNotification> : INotificationHandler<TDomainNotification>
        where TDomainNotification : DomainNotification
    {

    }
}
