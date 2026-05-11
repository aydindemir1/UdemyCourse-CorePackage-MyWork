using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Transport.RabbitMq
{
    public class PublisherChannelContext : IDisposable
    {
        private readonly ILogger<PublisherChannelContext> _logger;
        private readonly IPublisherChannelContextPool _publisherChannelContextPool;

        public IChannel Channel;

        public QueueReferences QueueReferences;

        public PublisherChannelContext(IPublisherChannelContextPool publisherChannelContextPool, ILogger<PublisherChannelContext> logger, IChannel channel, QueueReferences queueReferences)
        {
            _publisherChannelContextPool = publisherChannelContextPool ?? throw new ArgumentNullException(nameof(publisherChannelContextPool));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Channel = channel ?? throw new ArgumentNullException(nameof(channel));
            QueueReferences = queueReferences ?? throw new ArgumentNullException(nameof(queueReferences));
        }

        public void Dispose()
        {
            try
            {
                if (Channel.IsOpen)
                {
                    _publisherChannelContextPool.Return(this);
                    _logger.LogDebug("Channel context returned to pool for exchange '{ExchangeName}'", QueueReferences.ExchangeName);
                }
                else
                {
                    _logger.LogWarning("Attempted to return closed channel to pool for exchange '{ExchangeName}'", QueueReferences.ExchangeName);
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error occurred while returning channel context to pool for exchange '{ExchangeName}'", QueueReferences.ExchangeName);
            }
        }
    }
}
