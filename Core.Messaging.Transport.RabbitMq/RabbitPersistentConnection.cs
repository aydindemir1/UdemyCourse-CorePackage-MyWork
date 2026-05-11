using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Transport.RabbitMq
{
    public sealed class RabbitPersistentConnection(IConnectionFactory connectionFactory, ILogger<RabbitPersistentConnection> logger) : IDisposable, IBusConnection
    {
        private readonly IConnectionFactory _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        private IConnection? _connection;

        private bool _disposed;

        private readonly object semaphore = new();

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;


        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            _connection?.Dispose();
        }


        public async Task<IChannel> CreateChannelAsync()
        {
            await TryConnectAsync();

            if (!IsConnected)
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");

            return await _connection.CreateChannelAsync();
        }


        private Task TryConnectAsync()
        {
            lock (semaphore)
            {
                if (IsConnected) return Task.CompletedTask;

                var policy = Policy.Handle<Exception>().WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, timeSpan, context) =>
                {
                    _logger.LogError(ex, $"An exception occurred while opening RabbitMq connection : {ex.Message}");
                    return Task.CompletedTask;
                });
                _connection = policy.ExecuteAsync(async () => await _connectionFactory.CreateConnectionAsync()).GetAwaiter().GetResult();


                _connection.ConnectionShutdownAsync += OnConnectionShutdown;
                _connection.CallbackExceptionAsync += OnCallbackException;
                _connection.ConnectionBlockedAsync += OnConnectionBlocked;
            }
            return Task.CompletedTask;

        }


        private async Task OnConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            _logger.LogWarning("RabbitMq Connection shutdown. Reason {Reason}", e.ReplyText);

            await TryConnectAsync();
        }

        private async Task OnCallbackException(object? sender, CallbackExceptionEventArgs e)
        {
            _logger.LogWarning(e.Exception, "RabbitMq callback exception occurred");

            await TryConnectAsync();
        }

        private async Task OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
        {
            _logger.LogWarning("RabbitMq connection blocked. Reason: {Reason}", e.Reason);
            await TryConnectAsync();
        }


    }
}
