using Core.Abstractions.Events.External;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Messaging.Outbox
{
    public interface IOutboxService
    {
        Task<IEnumerable<OutboxMessage>> GetAllUnsentOutboxMessageAsync(EventType eventType = EventType.IntegrationEvent, CancellationToken cancellationToken = default);

        Task<IEnumerable<OutboxMessage>> GetAllOutboxMessagesAsync(EventType eventType = EventType.IntegrationEvent, CancellationToken cancellationToken = default);

        Task CleanProcessedAsync(CancellationToken cancellationToken = default);

        Task SaveAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);

        Task SaveAsync(IIntegrationEvent[] integrationEvents, CancellationToken cancellationToken = default);
        Task PublishUnsentOutboxMessagesAsync(CancellationToken cancellationToken = default);
    }
}
