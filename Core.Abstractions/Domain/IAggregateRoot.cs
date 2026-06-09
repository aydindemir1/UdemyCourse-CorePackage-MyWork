using Core.Abstractions.Events.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Domain
{
    public interface IAggregateRoot
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get; }

        void ClearDomainEvents();
    }
}
