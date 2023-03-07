using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Polly.Retry;
using KiotVietTimeSheet.Infrastructure.EventBus.Extensions;
using KiotVietTimeSheet.SharedKernel.EventBus;
using KiotVietTimeSheet.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace KiotVietTimeSheet.Infrastructure.EventBus.RabbitMQ
{
    public sealed class EventBusRabbitMq : IEventBus, IDisposable
    {
        #region Properties

        private const string BrokerName = "kiotviet_event_bus";
        private readonly IRabbitMqPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMq> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly int _retryCount;
        private readonly IServiceProvider _serviceProvider;
        private readonly bool _useScope;

        private IModel _consumerChannel;
        private string _queueName;

        #endregion

        #region Constructor

        public EventBusRabbitMq(
            IRabbitMqPersistentConnection persistentConnection,
            ILogger<EventBusRabbitMq> logger,
            IEventBusSubscriptionsManager subsManager,
            IServiceProvider serviceProvider,
            string queueName = null,
            int retryCount = 5,
            bool useScope = false,
            bool isCreateConsumer = true
        )
        {
            _serviceProvider = serviceProvider;
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _queueName = queueName;
            if (isCreateConsumer)
            {
                _consumerChannel = CreateConsumerChannel();
            }
            _retryCount = retryCount;
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
            _useScope = useScope;
        }

        #endregion

        #region Public methods

        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning($@"Could not publish event: {@event.Id} after {time.TotalSeconds:N1}s ({ex.Message})");
                });

            policy.Execute(() =>
            {
                using (var channel = _persistentConnection.CreateModel())
                {
                    var eventName = @event.GetType()
                        .Name;

                    channel.ExchangeDeclare(exchange: BrokerName,
                        type: "direct");

                    var message = JsonConvert.SerializeObject(@event);
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    // properties.DeliveryMode = 2; // persistent

                    channel.BasicPublish(exchange: BrokerName,
                        routingKey: eventName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                }
            });
        }

        public void Publish(Type eventType, string eventData)
        {
            var @event = JsonConvert.DeserializeObject(eventData, eventType) as IntegrationEvent;
            Publish(@event);
        }

        public void Subscribe<T, Th>()
            where T : IntegrationEvent
            where Th : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            DoInternalSubscription(eventName);
            _logger.LogInformation($@"Subscribing to event {eventName} with {typeof(Th).GetGenericTypeName()}");
            _subsManager.AddSubscription<T, Th>();
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            _logger.LogInformation($@"Unsubscribing from event {eventName}");
            _subsManager.RemoveSubscription<T, TH>();
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();
            _subsManager.Clear();
        }

        #endregion

        #region Private methods

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueUnbind(queue: _queueName,
                    exchange: BrokerName,
                    routingKey: eventName);

                if (!_subsManager.IsEmpty) return;
                _queueName = string.Empty;
                _consumerChannel.Close();
            }
        }

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (containsKey) return;

            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueBind(queue: _queueName,
                    exchange: BrokerName,
                    routingKey: eventName);
            }
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: BrokerName,
                type: "direct");

            channel.QueueDeclare(queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);


            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                await ReceivedEvent(channel, ea);
            };

            channel.BasicConsume(queue: _queueName,
                autoAck: false,
                consumer: consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        private async Task ReceivedEvent(IModel channel, BasicDeliverEventArgs ea)
        {
            var eventName = ea.RoutingKey;
            var message = Encoding.UTF8.GetString(ea.Body);
            var retryCount = 0;
            var retryAllow = 3;
            var logId = Guid.NewGuid().ToString("N").ToLower();

            var @event = JsonConvert.DeserializeObject<IntegrationEvent>(message);

            // Thêm LogContext
            using (LogContext.PushProperty("BuildVersion", Globals.BuildVersion))
            using (LogContext.PushProperty("LogId", logId))
            using (LogContext.PushProperty("EventName", eventName))
            using (LogContext.PushProperty("Kv.RetailerId", @event?.Context?.TenantId))
            using (LogContext.PushProperty("Kv.BranchId", @event?.Context?.BranchId))
            using (LogContext.PushProperty("Kv.UserId", @event?.Context?.UserId))
            using (LogContext.PushProperty("Kv.RetailerCode", @event?.Context?.RetailerCode))
            using (LogContext.PushProperty("Kv.GroupId", @event?.Context?.GroupId))
            {
                while (retryCount <= retryAllow)
                {
                    try
                    {
                        if (_useScope)
                        {
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var eventContextSvc = scope.ServiceProvider.GetRequiredService<IEventContextService>();
                                eventContextSvc.SetContext(new EventContext
                                {
                                    TenantId = @event.Context.TenantId,
                                    BranchId = @event.Context.BranchId,
                                    UserId = @event.Context.UserId,
                                    RetailerCode = @event.Context.RetailerCode,
                                    User = @event.Context.User,
                                    GroupId = @event.Context.GroupId
                                });
                                await ProcessEvent(scope.ServiceProvider, eventName, message);
                            }
                        }
                        else
                        {
                            await ProcessEvent(_serviceProvider, eventName, message);
                        }

                        channel.BasicAck(ea.DeliveryTag, multiple: false);
                        break;
                    }
                    catch (Exception ex)
                    {
                        // Error call external service (MHQL,...) --> requeue
                        if (ex.Message == Constant.TaskCanceledExceptionMessage)
                        {
                            _logger.LogError(ex, $"[LogId: {logId}] Requeue event by TaskCanceledExceptionMessage. MessageData: {message}");
                            channel.BasicNack(ea.DeliveryTag, multiple: false, true);
                            break;
                        }

                        // Retry too times --> Ignore event
                        if (retryCount == retryAllow)
                        {
                            _logger.LogError(ex, $"[LogId: {logId}] Error when consum event --> Ignore event. MessageData: {message}");
                            channel.BasicAck(ea.DeliveryTag, multiple: false);
                            break;
                        }

                        retryCount++;
                        _logger.LogError(ex, $"[LogId: {logId}] Retry consum event ({retryCount}/{retryAllow}). MessageData: {message}");
                        await Task.Delay(2000);
                    }
                }
            }
        }

        private async Task ProcessEvent(IServiceProvider provider, string eventName, string message)
        {
            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    _logger.LogDebug("event: " + eventName);
                    var handler = provider.GetRequiredService(subscription.HandlerType);
                    if (handler == null) continue;
                    var eventType = _subsManager.GetEventTypeByName(eventName);
                    var integrationEvent = JsonConvert.DeserializeObject(message, eventType, new JsonSerializerSettings()
                    {
                        ContractResolver = new PrivateResolver(),
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                    });
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    var method = concreteType.GetMethod("Handle")?.Invoke(handler, new[] { integrationEvent });
                    if (method == null) continue;
                    await (Task)method;
                }
            }
        }

        #endregion
    }
}
