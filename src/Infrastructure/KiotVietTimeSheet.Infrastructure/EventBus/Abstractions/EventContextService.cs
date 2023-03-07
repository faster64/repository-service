namespace KiotVietTimeSheet.Infrastructure.EventBus.Abstractions
{
    public class EventContextService : IEventContextService
    {
        public EventContext Context { get; private set; }
        private readonly object _obj = new object();

        public void SetContext(EventContext context)
        {
            lock (_obj)
            {
                Context = context;
            }
        }
    }
}
