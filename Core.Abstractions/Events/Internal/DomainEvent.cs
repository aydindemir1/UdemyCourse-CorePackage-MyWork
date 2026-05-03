using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Events.Internal
{
    public abstract record DomainEvent : Event, IDomainEvent { }
}
