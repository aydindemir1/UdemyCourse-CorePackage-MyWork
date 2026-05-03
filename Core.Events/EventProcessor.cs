using Core.Abstractions.Events;
using Core.Abstractions.Events.External;
using Core.Abstractions.Events.Internal;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Events
{
    public class EventProcessor : IEventProcessor, IDomainEventDispatcher
    {
        private readonly IIntegrationEventPublisher _integrationEventPublisher;
        private readonly IMediator _mediator;
        private readonly ILogger<EventProcessor> _logger;

        public EventProcessor(IMediator mediator, ILogger<EventProcessor> logger, IIntegrationEventPublisher? integrationEventPublisher = null)
        {
            _integrationEventPublisher = integrationEventPublisher;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task PublishAsync<TEvent>(TEvent @event, EventPublishingStrategy strategy, CancellationToken cancellationToken = default) where TEvent : IEvent
        {
            if (@event is IIntegrationEvent integrationEvent)
            {
                await _integrationEventPublisher.PublishAsync(integrationEvent, strategy, cancellationToken);
                return;
            }

        }

        public async Task PublishAsync<TEvent>(TEvent[] @events, EventPublishingStrategy strategy, CancellationToken cancellationToken = default) where TEvent : IEvent
        {
            foreach (var @event in @events)
            {
                await PublishAsync(@event, strategy, cancellationToken);
            }
        }

        public async Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
        {
            if (@event is IIntegrationEvent integrationEvent)
            {
                await _mediator.DispatchIntegrationEventAsync(integrationEvent, _logger, cancellationToken);
                return;
            }
            await _mediator.Publish(@event, cancellationToken);
        }

        public async Task DispatchAsync<TEvent>(TEvent[] @events, CancellationToken cancellationToken = default) where TEvent : IEvent
        {
            foreach (var @event in @events)
            {
                await DispatchAsync(@event, cancellationToken);
            }
        }

        public async Task DispatchAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
            => await DispatchAsync<IDomainEvent>(@event, cancellationToken);

        public async Task DispatchAsync(IDomainEvent[] events, CancellationToken cancellationToken = default)
            => await DispatchAsync<IDomainEvent>(events, cancellationToken);
    }
}
