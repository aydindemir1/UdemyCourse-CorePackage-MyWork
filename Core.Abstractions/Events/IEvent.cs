using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Events
{
    public interface IEvent : INotification
    {
        Guid EventId { get; }

        long EventVersion { get; }

        DateTime OccurredOn { get; }

        DateTimeOffset TimeStamp { get; }

        string EventType { get; }
    }
}
