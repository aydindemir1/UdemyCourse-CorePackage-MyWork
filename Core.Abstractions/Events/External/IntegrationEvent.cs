using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Events.External
{
    public abstract record IntegrationEvent : Event, IIntegrationEvent
    {
        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
    }
}
