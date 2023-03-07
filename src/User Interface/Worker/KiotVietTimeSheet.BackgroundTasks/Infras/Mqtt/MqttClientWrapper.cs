using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using Polly;
using System;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.BackgroundTasks.Infras.Mqtt
{
    public class MqttClientWrapper : IDisposable
    {
        private readonly ILogger<MqttClientWrapper> _logger;
        private readonly IManagedMqttClient _mqttClient;
        private readonly IManagedMqttClientOptions _mqttClientOptions;
        private bool disposedValue;

        public MqttClientWrapper(ILogger<MqttClientWrapper> logger, MqttClientConfig mqttClientConfig)
        {
            _logger = logger;
            _mqttClientOptions = MqttClientHelpers.BuildManagedClientOptions(mqttClientConfig);
            _mqttClient = new MqttFactory().CreateManagedMqttClient();
        }

        public async Task PublishAsync(MqttClientPublishMessage message)
        {
            var policy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(2),
                    onRetry: (ex, time, count, context) =>
                    {
                        _logger.LogError(ex, $@"Retry attempt {count}. Could not publish to mqtt topic {message.Topic} after {time.TotalSeconds:N1}s ({ex.Message})");
                    });

            await policy.ExecuteAsync(async () =>
            {
                if (!_mqttClient.IsConnected)
                    throw new MqttClientException("Client is disconnected");

                var mqttMsg = MqttClientHelpers.BuildPublishMessage(message);
                await _mqttClient.PublishAsync(mqttMsg);
            });
        }

        public async Task ForcePublishAsync(MqttClientPublishMessage message)
        {
            var policy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(2),
                    onRetry: (ex, time, count, context) =>
                    {
                        _logger.LogError(ex, $@"Retry attempt {count}. Could not publish to mqtt topic {message.Topic} after {time.TotalSeconds:N1}s ({ex.Message})");
                    });

            await policy.ExecuteAsync(async () =>
            {
                if (!_mqttClient.IsStarted)
                    throw new MqttClientException("Client is not started");

                var mqttMsg = MqttClientHelpers.BuildPublishMessage(message);
                await _mqttClient.PublishAsync(mqttMsg);
            });
        }

        public async Task StartAsync()
        {
            await _mqttClient.StartAsync(_mqttClientOptions);
        }

        public async Task StopAsync()
        {
            await _mqttClient.StopAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    _mqttClient.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
