using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Events.External
{
    public interface IIntegrationEventPublisher
    {
        Task PublishAsync(IIntegrationEvent @event, EventPublishingStrategy strategy, CancellationToken cancellationToken = default);
        Task PublishAsync(IIntegrationEvent[] @event, EventPublishingStrategy strategy, CancellationToken cancellationToken = default);
    }
}
