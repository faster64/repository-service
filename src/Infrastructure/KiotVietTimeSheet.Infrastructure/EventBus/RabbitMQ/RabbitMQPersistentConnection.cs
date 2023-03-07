using System;
using System.IO;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace KiotVietTimeSheet.Infrastructure.EventBus.RabbitMQ
{
    public sealed class RabbitMqPersistentConnection : IRabbitMqPersistentConnection
    {
        #region Properties

        private readonly IConnectionFactory _connectionFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitMqPersistentConnection> _logger;
        private readonly int _retryCount;
        private IConnection _connection;
        private bool _disposed;
        private bool _isRetry;
        private readonly object _syncRoot = new object();
        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        #endregion

        #region Constructor

        public RabbitMqPersistentConnection(IConnectionFactory connectionFactory,
                                            ILogger<RabbitMqPersistentConnection> logger,
                                            int retryCount = 5,
                                            IServiceProvider serviceProvider = null)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _retryCount = retryCount;
            _serviceProvider = serviceProvider;
        }

        #endregion

        #region Public methods

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, ex.ToString());
            }
        }

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            lock (_syncRoot)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                        {
                            _logger.LogWarning($@"RabbitMQ Client could not connect after {time.TotalSeconds:N1}s ({ex.Message})");
                            _isRetry = true;
                        }
                    );

                policy.Execute(() =>
                {
                    _connection = _connectionFactory.CreateConnection();
                });

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;
                    if (_serviceProvider != null && _isRetry && _serviceProvider.GetService(typeof(IApplicationLifetime)) is IApplicationLifetime lifeTime)
                    {
                        // Restart service to subscribe event again when rabbit is incorrupt
                        lifeTime.StopApplication();
                    }
                    _logger.LogInformation($@"RabbitMQ Client acquired a persistent connection to '{_connection.Endpoint.HostName}' and is subscribed to failure events");

                    return true;
                }
                else
                {
                    _logger.LogError("FATAL ERROR: RabbitMQ connections could not be created and opened");
                    return false;
                }
            }
        }

        #endregion

        #region Private methods

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;
            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
            _isRetry = true;
            TryConnect();
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;
            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
            _isRetry = true;
            TryConnect();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;
            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
            _isRetry = true;
            TryConnect();
        }

        #endregion
    }
}
