using Core.Abstractions.Events.External;
using Core.Abstractions.Messaging.Serialization;
using Core.Abstractions.Messaging.Transport;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Transport.RabbitMq.Producers
{
    public class RabbitMqProducer : IEventBusPublisher
    {
        private readonly IPublisherChannelFactory _publisherChannelFactory;
        private readonly IMessageSerializer _messageSerializer;
        private readonly ILogger<RabbitMqProducer> _logger;

        public RabbitMqProducer(
            IPublisherChannelFactory publisherChannelFactory,
            IMessageSerializer messageSerializer,
            ILogger<RabbitMqProducer> logger)
        {
            _publisherChannelFactory = publisherChannelFactory ?? throw new ArgumentNullException(nameof(publisherChannelFactory));
            _messageSerializer = messageSerializer ?? throw new ArgumentNullException(nameof(messageSerializer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IIntegrationEvent
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            using var context = await _publisherChannelFactory.CreateAsync(@event);

            var encodedMessage = _messageSerializer.Serialize(@event);

            var headers = new Dictionary<string, object>
            {
                {HeaderNames.MessageType,Encoding.UTF8.GetBytes(@event.EventType) }
            };

            if (!string.IsNullOrEmpty(@event.CorrelationId))
            {
                headers.Add(HeaderNames.CorrelationId, Encoding.UTF8.GetBytes(@event.CorrelationId));
            }

            var properties = new BasicProperties
            {
                Persistent = true,
                // OpenTelemetry semantic convention standardı olan "messaging.message.conversation_id" tag adıyla
                //otomatik ekler.
                CorrelationId = @event.CorrelationId ?? string.Empty,
                Headers = headers

            };

            var policy = Policy.Handle<Exception>().WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                _logger.LogWarning(ex, "Could not publish message '{MessageId}' to Exchange '{ExchangeName}' after {Timeout}s : {ExceptionMessage}", @event.EventId, context.QueueReferences.ExchangeName, $"{time.TotalSeconds:n1}", ex.Message);
            });

            policy.Execute(() =>
            {
                context.Channel.BasicPublishAsync(
                    exchange: context.QueueReferences.ExchangeName,
                    routingKey: context.QueueReferences.RoutingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: Encoding.UTF8.GetBytes(encodedMessage));

                _logger.LogInformation("message '{MessageId}' published to Exchange '{ExchangeName}'", @event.EventId, context.QueueReferences.ExchangeName);
            });

        }
    }
}
