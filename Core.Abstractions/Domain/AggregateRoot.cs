using Core.Abstractions.Events.Internal;
using Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Domain
{
    public abstract class AggregateRoot<TId> : BaseEntity<TId>, IAggregateRoot
    {
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent @event)
            => _domainEvents.Add(@event);

        public void ClearDomainEvents()
            => _domainEvents.Clear();
    }
}
