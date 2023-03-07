using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using KiotVietTimeSheet.Infrastructure.EventBus.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Polly;
using RabbitMQ.Client.Exceptions;
using Polly.Retry;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types
{
    public class RealtimeProcess
    {
        private const string BrokerName = "kiotviet_event_bus";
        private readonly string _queueName;
        private readonly List<string> _exchangeName;
        private readonly int _retryCount;

        private readonly IRabbitMqPersistentConnection _rabbitMqPersistentConnection;
        private readonly ILogger<RealtimeProcess> _logger;


        public RealtimeProcess(
            IRabbitMqPersistentConnection rabbitMqPersistentConnection,
            ILogger<RealtimeProcess> logger,
            IConfiguration configuration
        )
        {
            _rabbitMqPersistentConnection = rabbitMqPersistentConnection;
            _logger = logger;
            _retryCount = 5;
            _queueName = configuration.GetValue<string>("QueueNameTimeSheetPaySheetRealtime");
            _exchangeName = configuration.GetSection("ExchangeTimeSheetPaySheetKey").Get<List<string>>();
            var isCreateQueue = bool.Parse(configuration.GetValue<string>("IsCreateTimeSheetPaySheetQueue"));
            if (isCreateQueue)
                DoCreateQueue();
        }

        #region Private Method

        public void SendEventData(IntegrationEvent @event, string eventType)
        {
            if (!_rabbitMqPersistentConnection.IsConnected)
            {
                _rabbitMqPersistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning($@"Could not publish event: {@event.Id} after {time.TotalSeconds:N1}s ({ex.Message})");
                });

            policy.Execute(() =>
            {
                using (var channel = _rabbitMqPersistentConnection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: BrokerName,
                        type: "direct");

                    var message = JsonConvert.SerializeObject(@event, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();

                    channel.BasicPublish(exchange: BrokerName,
                        routingKey: eventType,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                }
            });
        }


        private void DoCreateQueue()
        {
            if (!_rabbitMqPersistentConnection.IsConnected)
            {
                _rabbitMqPersistentConnection.TryConnect();
            }

            using (var channel = _rabbitMqPersistentConnection.CreateModel())
            {
                channel.QueueDeclare(_queueName, true, false, false, null);
                var integrationSockets = _exchangeName;

                foreach (var integrationSocket in integrationSockets)
                {
                    channel.QueueBind(queue: _queueName,
                        exchange: BrokerName,
                        routingKey: integrationSocket);
                }
            }
        }
        #endregion


    }
}
