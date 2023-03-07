namespace KiotVietTimeSheet.Infrastructure.EventBus.Abstractions
{
    public interface IEventContextService
    {
        void SetContext(EventContext context);

        EventContext Context { get; }
    }
}
