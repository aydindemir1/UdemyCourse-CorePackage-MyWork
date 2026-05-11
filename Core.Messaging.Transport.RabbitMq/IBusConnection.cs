using RabbitMQ.Client;
using System.Threading.Channels;

namespace Core.Messaging.Transport.RabbitMq
{
    public interface IBusConnection
    {
        bool IsConnected { get; }

        Task<IChannel> CreateChannelAsync();
    }
}
