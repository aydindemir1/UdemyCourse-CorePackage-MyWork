using Ardalis.GuardClauses;
using Core.Abstractions.Events;
using Core.Abstractions.Events.External;
using Core.Events.External;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Events.External
{
    public class IntegrationEventPublisher : IIntegrationEventPublisher
    {
        private readonly TransactionalEventPublisher _transactionalEventPublisher;
        private readonly VolatileEventPublisher _volatileEventPublisher;

        public IntegrationEventPublisher(TransactionalEventPublisher transactionalEventPublisher, VolatileEventPublisher volatileEventPublisher)
        {
            _transactionalEventPublisher = transactionalEventPublisher;
            _volatileEventPublisher = volatileEventPublisher;
        }

        public Task PublishAsync(IIntegrationEvent @event, EventPublishingStrategy strategy, CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(@event, nameof(@event));

            return strategy switch
            {
                EventPublishingStrategy.Volatile => _volatileEventPublisher.PublishAsync(@event, cancellationToken),
                _ => _transactionalEventPublisher.PublishAsync(@event, cancellationToken)
            };
        }

        public async Task PublishAsync(IIntegrationEvent[] @events, EventPublishingStrategy strategy, CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(@events, nameof(@events));

            foreach (var @event in @events)
            {
                await PublishAsync(@event, strategy, cancellationToken);
            }
        }
    }
}
