using Core.Abstractions.Messaging.Transport;
using Core.Messaging.Transport.RabbitMq.Consumers;
using Core.Messaging.Transport.RabbitMq.Producers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Transport.RabbitMq
{
    public static class RabbitMqExtensions
    {
        public static IServiceCollection AddRabbitMqTransport(this IServiceCollection services, IConfiguration configuration, Action<RabbitConfiguration> configurator = null)
        {
            services.AddSingleton<IQueueReferenceFactory, QueueReferenceFactory>();

            services.AddSingleton<IMessageParser, MessageParser>();

            services.AddSingleton<IEventBusPublisher, RabbitMqProducer>();

            services.AddSingleton<IEventBusSubscriber, RabbitMqConsumer>();

            services.AddSingleton<IPublisherChannelContextPool, PublisherChannelContextPool>();

            services.AddSingleton<IPublisherChannelFactory, PublisherChannelFactory>();

            services.Configure<RabbitConfiguration>(configuration.GetSection("RabbitConfiguration"));

            if (configurator is { })
                services.Configure(nameof(RabbitConfiguration), configurator);

            var config = configuration.GetSection("RabbitConfiguration").Get<RabbitConfiguration>();

            services.AddSingleton<IConnectionFactory>(ctx =>
            {
                var connectionFactory = new ConnectionFactory
                {
                    HostName = config.HostName,
                    UserName = config.UserName,
                    Password = config.Password,
                    Port = config.Port
                };
                return connectionFactory;
            });

            services.AddSingleton<IBusConnection, RabbitPersistentConnection>();
            services.AddSingleton(config);
            return services;

        }
    }
}
