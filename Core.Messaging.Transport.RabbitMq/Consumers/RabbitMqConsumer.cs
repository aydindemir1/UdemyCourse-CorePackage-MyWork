using Core.Abstractions.Events;
using Core.Abstractions.Events.External;
using Core.Abstractions.Messaging.Transport;
using Core.Extensions.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Core.Messaging.Transport.RabbitMq.Consumers
{
    public class RabbitMqConsumer : IEventBusSubscriber
    {
        private readonly IBusConnection _busConnection;
        private readonly IMessageParser _messageParser;
        private readonly ILogger<RabbitMqConsumer> _logger;
        private readonly RabbitConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly List<IChannel> _channels = new();

        public RabbitMqConsumer(IBusConnection busConnection, IMessageParser messageParser, ILogger<RabbitMqConsumer> logger, RabbitConfiguration configuration, IServiceProvider serviceProvider)
        {
            _busConnection = busConnection ?? throw new ArgumentNullException(nameof(busConnection));
            _messageParser = messageParser ?? throw new ArgumentNullException(nameof(messageParser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }


        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            var messageTypes = AppDomain.CurrentDomain.GetAssemblies().GetHandledIntegrationEventTypes();

            var factory = _serviceProvider.GetRequiredService<IQueueReferenceFactory>();

            foreach (var messageType in messageTypes)
            {
                MethodInfo methodInfo = typeof(IQueueReferenceFactory).GetMethod("Create");
                MethodInfo generic = methodInfo.MakeGenericMethod(messageType);

                var queueReferences = generic.Invoke(factory, new object[] { null }) as QueueReferences;

                var channel = await InitChannelAsync(queueReferences);

                _channels.Add(channel);

                await InitSubsriptionAsync(queueReferences, channel);
            }

        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            foreach (var channel in _channels)
            {
                await StopChannelAsync(channel);
            }
        }

        private async Task InitSubsriptionAsync(QueueReferences queueReferences, IChannel channel)
        {
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += OnMessageReceivedAsync;

            _logger.LogInformation("Initilizing subscription for queue {queueName}", queueReferences.QueueName);

            await channel.BasicConsumeAsync(queue: queueReferences.QueueName, autoAck: false, consumer: consumer);
        }

        private async Task<IChannel> InitChannelAsync(QueueReferences queueReferences)
        {
            var channel = await _busConnection.CreateChannelAsync();
            _logger.LogInformation("Initilizing queue '{QueueName}' on exchange '{ExchangeName}'", queueReferences.QueueName, queueReferences.ExchangeName);


            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: _configuration.PrefetchCount, global: false);

            await channel.ExchangeDeclareAsync(exchange: queueReferences.ExchangeName, type: ExchangeType.Topic);

            await channel.QueueDeclareAsync(queue: queueReferences.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object>
        {
            {Headers.XDeadLetterExchange,queueReferences.DeadLetterExchangeName },
            {Headers.XDeadLetterRoutingKey,queueReferences.DeadLetterQueue   }
        });

            await channel.QueueBindAsync(queue: queueReferences.QueueName, exchange: queueReferences.ExchangeName, routingKey: queueReferences.RoutingKey, arguments: null);

            channel.CallbackExceptionAsync += OnChannelExceptionAsync;
            return channel;
        }

        private Task OnChannelExceptionAsync(object sender, CallbackExceptionEventArgs eventArgs)
        {
            _logger.LogError(eventArgs.Exception, "RabbitMq channel exception : {Message}", eventArgs.Exception.Message);
            return Task.CompletedTask;
        }

        private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
        {
            var consumer = sender as AsyncEventingBasicConsumer;
            var channel = consumer?.Channel;

            IIntegrationEvent message;
            try
            {
                message = _messageParser.Resolve(eventArgs.BasicProperties, eventArgs.Body.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decoding queue message from exchange '{ExchangeName}' : {ExceptionMessage}", eventArgs.Exchange, ex.Message);
                if (channel != null)
                    await channel.BasicRejectAsync(eventArgs.DeliveryTag, requeue: false);
                return;
            }


            try
            {
                using var scope = _serviceProvider.CreateScope();

                var eventProcessor = scope.ServiceProvider.GetService<IEventProcessor>();

                await eventProcessor.DispatchAsync(message, default);

                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);

            }
            catch (Exception ex)
            {
                await HandleConsumerExceptionAsync(ex, eventArgs, channel, message);
            }
        }

        private async Task HandleConsumerExceptionAsync(Exception exception, BasicDeliverEventArgs deliverProps, IChannel channel, IIntegrationEvent message)
        {
            _logger.LogWarning("Error processing {MessageId}. Rejecting to DLQ..", message.EventId);

            await channel.BasicRejectAsync(deliverProps.DeliveryTag, requeue: false);
        }

        private async Task StopChannelAsync(IChannel channel)
        {
            if (channel is null) return;
            channel.CallbackExceptionAsync -= OnChannelExceptionAsync;
            if (channel.IsOpen)
                await channel.CloseAsync();
            channel.Dispose();
        }

    }
}
