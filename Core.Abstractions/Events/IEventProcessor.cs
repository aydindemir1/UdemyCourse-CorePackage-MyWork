using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Events
{
    public interface IEventProcessor
    {
        Task PublishAsync<TEvent>(TEvent @event, EventPublishingStrategy strategy, CancellationToken cancellationToken = default)
            where TEvent : IEvent;

        Task PublishAsync<TEvent>(TEvent[] @event, EventPublishingStrategy strategy, CancellationToken cancellationToken = default)
            where TEvent : IEvent;

        Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IEvent;

        Task DispatchAsync<TEvent>(TEvent[] @event, CancellationToken cancellationToken = default)
           where TEvent : IEvent;
    }
}
