using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Messaging.Transport
{
    public interface IEventBusSubscriber
    {
        Task StartAsync(CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
