using Core.Abstractions.Events.External;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Messaging.Transport
{
    public interface IEventBusPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IIntegrationEvent;
    }
}
