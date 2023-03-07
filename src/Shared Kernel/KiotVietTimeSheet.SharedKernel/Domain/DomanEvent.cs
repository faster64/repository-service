using KiotVietTimeSheet.SharedKernel.Models;
using System;

namespace KiotVietTimeSheet.SharedKernel.Domain
{
    public class DomainEvent : Message
    {
        public Guid AggregateId { get; protected set; }
        public DateTime Timestamp { get; protected set; }
        public string EventType { get; protected set; }
        public string Context { get; set; }

        protected DomainEvent()
        {
            AggregateId = Guid.NewGuid();
            Timestamp = DateTime.Now;
            EventType = GetType().Name;
        }
    }
}
