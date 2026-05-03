using Ardalis.GuardClauses;
using Core.Abstractions.Events;
using Core.Abstractions.Events.External;
using Core.Abstractions.Events.Internal;
using Core.Abstractions.Messaging.Serialization;
using Core.Events.External;
using Core.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.Events
{
    public static class EventExtensions
    {
        public static IServiceCollection AddDomainEvent(this IServiceCollection services)
        {
            services.AddScoped<EventProcessor>();
            services.AddScoped<IDomainEventDispatcher>(sp => sp.GetRequiredService<EventProcessor>());
            return services;
        }

        public static IServiceCollection AddEvent(this IServiceCollection services)
        {
            services.AddDomainEvent();

            services.AddScoped<IEventProcessor>(sp => sp.GetRequiredService<EventProcessor>());

            services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>()
                    .AddScoped<TransactionalEventPublisher>()
                    .AddScoped<VolatileEventPublisher>();
            return services;
        }

        public static async Task DispatchIntegrationEventAsync(this IMediator mediator, IIntegrationEvent integrationEvent, ILogger logger, CancellationToken cancellation = default)
        {
            Guard.Against.Null(integrationEvent, nameof(integrationEvent));

            var serializer = ServiceActivator.GetRequiredService<IMessageSerializer>();

            await mediator.Publish(integrationEvent, cancellation);

            logger.LogDebug("Published integration event: {IntegrationEventName} with payload {IntegrationEventContent}", integrationEvent.GetType().FullName, serializer.Serialize(integrationEvent));
        }

        public static Task DispatchIntegrationEventAsync(this IMediator mediator, IReadOnlyList<IIntegrationEvent> integrationEvents, ILogger logger, CancellationToken cancellation = default)
        {
            Guard.Against.Null(integrationEvents, nameof(integrationEvents));
            var tasks = integrationEvents.Select(evt => mediator.DispatchIntegrationEventAsync(evt, logger, cancellation));
            return Task.WhenAll(tasks);
        }

    }
}
