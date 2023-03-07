using KiotVietTimeSheet.SharedKernel.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiotVietTimeSheet.SharedKernel.Models
{
    public abstract class BaseEntity
    {
        private List<DomainEvent> _domainEvents;

        [NotMapped]
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

        protected void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents = _domainEvents ?? new List<DomainEvent>();
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
    }
}
