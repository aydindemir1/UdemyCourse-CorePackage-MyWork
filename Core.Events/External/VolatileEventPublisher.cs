using Ardalis.GuardClauses;
using Core.Abstractions.Events.External;
using Core.Abstractions.Messaging.Transport;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Events.External
{
    public class VolatileEventPublisher
    {
        private readonly IEventBusPublisher _eventBusPublisher;

        public VolatileEventPublisher(IEventBusPublisher eventBusPublisher)
        {
            _eventBusPublisher = eventBusPublisher;
        }

        public Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(integrationEvent, nameof(integrationEvent));
            return _eventBusPublisher.PublishAsync(integrationEvent, cancellationToken);
        }

        public async Task PublishAsync(IIntegrationEvent[] integrationEvents, CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(integrationEvents, nameof(integrationEvents));
            foreach (var integration in integrationEvents)
            {
                await PublishAsync(integration, cancellationToken);
            }
        }

    }
}
