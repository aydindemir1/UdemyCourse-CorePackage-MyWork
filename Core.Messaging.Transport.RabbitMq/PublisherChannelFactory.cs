using Core.Abstractions.Events.External;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Transport.RabbitMq
{
    public class PublisherChannelFactory : IPublisherChannelFactory
    {
        private readonly IPublisherChannelContextPool _publisherChannelContextPool;
        private readonly IQueueReferenceFactory _queueReferenceFactory;

        public PublisherChannelFactory(IPublisherChannelContextPool publisherChannelContextPool, IQueueReferenceFactory queueReferenceFactory)
        {
            _publisherChannelContextPool = publisherChannelContextPool ?? throw new ArgumentNullException(nameof(publisherChannelContextPool));
            _queueReferenceFactory = queueReferenceFactory ?? throw new ArgumentNullException(nameof(queueReferenceFactory));
        }

        public Task<PublisherChannelContext> CreateAsync(IIntegrationEvent message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            var queueReferences = _queueReferenceFactory.Create((dynamic)message);

            var result = _publisherChannelContextPool.GetAsync(queueReferences);

            return result;
        }
    }
}
