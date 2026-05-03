using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Events.Internal
{
    public interface IDomainEventHandler<in TDomainEvent> : IEventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
    { }
}
