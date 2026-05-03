using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Events
{
    public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IEvent;
}
