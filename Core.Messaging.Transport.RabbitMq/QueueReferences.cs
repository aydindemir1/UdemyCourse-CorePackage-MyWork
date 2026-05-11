using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Transport.RabbitMq
{
    public record QueueReferences(string ExchangeName, string QueueName, string RoutingKey, string DeadLetterExchangeName, string DeadLetterQueue)
    {
        public string RetryExchangeName => $"{ExchangeName}.retry";
        public string RetryQueueName => $"{QueueName}.retry";
    }
}
