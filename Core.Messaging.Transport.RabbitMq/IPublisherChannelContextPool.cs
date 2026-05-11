using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Transport.RabbitMq
{
    public interface IPublisherChannelContextPool
    {
        Task<PublisherChannelContext> GetAsync(QueueReferences queueReferences);

        void Return(PublisherChannelContext channelContext);
    }
}
