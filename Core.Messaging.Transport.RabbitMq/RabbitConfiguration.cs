using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Transport.RabbitMq
{
    public class RabbitConfiguration
    {
        public string HostName { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
        public ushort PrefetchCount { get; set; } = 30;
    }
}
