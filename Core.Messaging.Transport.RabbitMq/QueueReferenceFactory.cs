using Core.Abstractions.Events.External;
using Core.Extensions.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;



namespace Core.Messaging.Transport.RabbitMq
{
    public class QueueReferenceFactory : IQueueReferenceFactory
    {
        private readonly ConcurrentDictionary<Type, QueueReferences> _queueReferenceCache = new();

        private readonly Func<Type, QueueReferences> _defaultCreator;

        private readonly IServiceProvider _serviceProvider;

        public QueueReferenceFactory(IServiceProvider serviceProvider, Func<Type, QueueReferences> defaultCreator = null)
        {
            _serviceProvider = serviceProvider;
            _defaultCreator = defaultCreator ?? ((Func<Type, QueueReferences>)(messageType =>
            {
                var exchangeName = messageType.Name.ToLower();

                var isEvent = messageType.IsEvent();

                var queueName = isEvent ? $"{exchangeName}.{messageType.Assembly.GetName().Name}.workers"
                : $"{exchangeName}.workers";

                var dlExchangeName = exchangeName + ".dead";

                var dlQueueName = isEvent ? $"{dlExchangeName}.{messageType.Assembly.GetName().Name}.workers"
                : $"{dlExchangeName}.workers";

                var routingKey = isEvent ? exchangeName : queueName;

                return new QueueReferences(exchangeName, queueName, routingKey, dlExchangeName, dlQueueName);

            }));
        }

        public QueueReferences Create<TMessage>(TMessage message = default) where TMessage : IIntegrationEvent
            => _queueReferenceCache.GetOrAdd(typeof(TMessage), k => CreateCore<TMessage>());

        private QueueReferences CreateCore<TMessage>() where TMessage : IIntegrationEvent
        {
            var creator = _serviceProvider.GetService<QueueReferencesPolicy<TMessage>>();
            return (creator is null) ? _defaultCreator(typeof(TMessage)) : creator();
        }

        public delegate QueueReferences QueueReferencesPolicy<TMessage>()
            where TMessage : IIntegrationEvent;
    }
}
