using Core.Abstractions.Messaging.Serialization;
using Core.Messaging.Background;
using Core.Messaging.Serialization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging
{
    public static class MessagingExtensions
    {
        public static IServiceCollection AddMessagingCore(this IServiceCollection services)
        {
            services.AddHostedService<OutboxProcessorBackgroundService>();

            return services;
        }

        public static IServiceCollection AddMessagingSerializer(this IServiceCollection services)
        {
            services.AddSingleton<IMessageSerializer, NewtonsoftJsonMessageSerializer>();
            return services;
        }

        public static IServiceCollection AddHostedSubscriber(this IServiceCollection services)
        {
            services.AddHostedService<SubscribersBackgroundService>();
            return services;
        }
    }
}
