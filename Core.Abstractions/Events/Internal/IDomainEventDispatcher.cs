using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Events.Internal
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IDomainEvent @event, CancellationToken cancellationToken = default);
        Task DispatchAsync(IDomainEvent[] events, CancellationToken cancellationToken = default);
    }
}
