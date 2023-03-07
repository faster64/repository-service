using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog.Context;
using System;
using System.Diagnostics;
using System.Text;

namespace KiotVietTimeSheet.Infrastructure.EventBus.RabbitMQ
{
    public class RabbitMqProducer : IEventBus
    {
        private const string BrokerName = "kiotviet_event_bus";

        private readonly DefaultObjectPool<IModel> _objectPool;
        private readonly ILogger<RabbitMqProducer> _logger;

        public RabbitMqProducer(IPooledObjectPolicy<IModel> objectPolicy, ILogger<RabbitMqProducer> logger)
        {
            _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
            _logger = logger;
        }

        public void Publish(IntegrationEvent @event)
        {
            var stopWatch = Stopwatch.StartNew();
            if (@event == null)
                return;

            // Get channel from pool
            var channel = _objectPool.Get();
            try
            {
                var eventName = @event.GetType()
                        .Name;

                channel.ExchangeDeclare(exchange: BrokerName,
                    type: "direct");

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();

                channel.BasicPublish(exchange: BrokerName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            }
            finally
            {
                stopWatch.Stop();
                _objectPool.Return(channel);
                using (LogContext.PushProperty("FeatureName", "RabbitMQProducerPublish"))
                using (LogContext.PushProperty("ExcutionTimeMs", stopWatch.ElapsedMilliseconds))
                {
                    _logger.LogInformation($"[FeatureProcessExecutionTime] take {stopWatch.ElapsedMilliseconds}ms");
                }
            }
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
            throw new NotImplementedException();
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }
    }
}
