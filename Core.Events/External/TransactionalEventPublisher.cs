using Ardalis.GuardClauses;
using Core.Abstractions.Events.External;
using Core.Abstractions.Messaging.Outbox;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Events.External
{
    public class TransactionalEventPublisher
    {
        private readonly IServiceProvider _serviceProvider;

        public TransactionalEventPublisher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(integrationEvent, nameof(integrationEvent));
            var outboxService = _serviceProvider.GetRequiredService<IOutboxService>();
            return outboxService.SaveAsync(integrationEvent, cancellationToken);
        }

        public async Task PublishAsync(IIntegrationEvent[] integrationEvents, CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(integrationEvents, nameof(integrationEvents));
            foreach (var integrationEvent in integrationEvents)
            {
                await PublishAsync(integrationEvent, cancellationToken);
            }
        }
    }
}
