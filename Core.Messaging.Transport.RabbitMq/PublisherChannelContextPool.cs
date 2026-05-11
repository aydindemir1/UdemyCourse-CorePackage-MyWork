using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Transport.RabbitMq
{
    public class PublisherChannelContextPool : IPublisherChannelContextPool, IDisposable
    {

        private readonly IBusConnection _busConnection;
        private readonly ILogger<PublisherChannelContext> _logger;

        private readonly ConcurrentDictionary<string, ConcurrentBag<PublisherChannelContext>> _pools = new();

        public PublisherChannelContextPool(IBusConnection busConnection, ILogger<PublisherChannelContext> logger)
        {
            _busConnection = busConnection ?? throw new ArgumentNullException(nameof(busConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PublisherChannelContext> GetAsync(QueueReferences queueReferences)
        {
            if (queueReferences == null)
                throw new ArgumentNullException(nameof(queueReferences));

            var pool = _pools.GetOrAdd(queueReferences.ExchangeName, _ => new());

            if (!pool.TryTake(out var ctx))
            {
                var channel = await _busConnection.CreateChannelAsync();

                await channel.ExchangeDeclareAsync(exchange: queueReferences.ExchangeName, type: ExchangeType.Topic);

                ctx = new PublisherChannelContext(this, _logger, channel, queueReferences);
                _logger.LogDebug("Created new channel context for exchange '{ExchangeName}'", queueReferences.ExchangeName);
            }
            else
            {
                _logger.LogDebug("Retrieved channel context from pool for exchange '{ExchangeName}'", queueReferences.ExchangeName);
            }
            return ctx;
        }

        public void Return(PublisherChannelContext channelContext)
        {
            if (channelContext == null)
                throw new ArgumentNullException(nameof(channelContext));
            if (channelContext.Channel.IsClosed)
                return;
            var pool = _pools.GetOrAdd(channelContext.QueueReferences.ExchangeName, _ => new());

            pool.Add(channelContext);
        }

        public void Dispose()
        {
            foreach (var pool in _pools.Values)
            {
                foreach (var ctx in pool)
                {
                    if (ctx.Channel.IsOpen)
                        ctx.Channel.CloseAsync();
                    ctx.Channel.Dispose();
                }
            }
            _pools.Clear();
        }

        public int GetAvailableCount() => _pools.Sum(p => p.Value.Count);

    }
}
