using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Events
{
    public abstract record Event : IEvent
    {
        public Guid EventId { get; protected set; }

        public long EventVersion { get; protected set; }

        public DateTime OccurredOn { get; protected set; }

        public DateTimeOffset TimeStamp { get; protected set; }

        public string EventType => GetType().AssemblyQualifiedName;

        protected Event()
        {
            EventId = Guid.NewGuid();
            EventVersion = 1;
            OccurredOn = DateTime.UtcNow;
            TimeStamp = DateTimeOffset.UtcNow;
        }
    }
}
