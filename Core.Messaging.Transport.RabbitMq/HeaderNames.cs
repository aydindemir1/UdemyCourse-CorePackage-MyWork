using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Transport.RabbitMq
{
    public static class HeaderNames
    {
        public const string MessageType = "message-type";

        public const string XDeadLetterExchange = "x-dead-letter-exchange";

        public const string XDeadLetterRoutingKey = "x-dead-letter-routing-key";

        public const string CorrelationId = "correlation-id";
    }
}
