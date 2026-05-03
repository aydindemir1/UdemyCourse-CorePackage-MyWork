using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Messaging.Outbox
{
    public class OutboxMessage
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public DateTime OccurredOn { get; private set; }
        public string Type { get; private set; }
        public string Data { get; private set; }
        public DateTime? ProcessedOn { get; private set; }
        public EventType EventType { get; private set; }
        public string? CorrelationId { get; private set; }
        public string EventId { get; set; }

        public OutboxMessage(
            string eventId,
            DateTime occurredOn,
            string type,
            string name,
            string data,
            EventType eventType,
            string correlationId)
        {
            EventId = eventId;
            OccurredOn = occurredOn;
            Type = type;
            Name = name;
            Data = data;
            EventType = eventType;
            CorrelationId = correlationId;
        }

        public void MarkAsProcessed()
        {
            ProcessedOn = DateTime.UtcNow;
        }
    }
}
