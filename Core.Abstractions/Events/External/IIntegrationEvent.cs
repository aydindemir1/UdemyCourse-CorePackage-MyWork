using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Events.External
{
    public interface IIntegrationEvent : IEvent
    {
        string CorrelationId { get; }
    }
}
