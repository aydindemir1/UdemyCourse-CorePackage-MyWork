using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Events.External
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
    {
    }
}
