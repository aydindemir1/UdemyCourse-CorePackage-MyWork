using Core.Abstractions.Events.External;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Transport.RabbitMq
{
    public interface IQueueReferenceFactory
    {
        QueueReferences Create<TMessage>(TMessage message = default)
            where TMessage : IIntegrationEvent;
    }
}
