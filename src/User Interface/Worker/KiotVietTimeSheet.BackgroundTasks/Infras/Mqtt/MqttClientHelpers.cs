using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using System;

namespace KiotVietTimeSheet.BackgroundTasks.Infras.Mqtt
{
    public class MqttClientHelpers
    {
        protected MqttClientHelpers() { }
        public static MqttApplicationMessage BuildPublishMessage(MqttClientPublishMessage message)
        {
            var msgBuilder = new MqttApplicationMessageBuilder()
                .WithTopic(message.Topic)
                .WithPayload(message.Message)
                .WithRetainFlag(message.Retain)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)message.Qos);

            return msgBuilder.Build();
        }

        public static IManagedMqttClientOptions BuildManagedClientOptions(MqttClientConfig mqttClientConfig)
        {
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(mqttClientConfig.ReConnectPrediodInSecond))
                .WithClientOptions(BuildOptions(mqttClientConfig));

            return options.Build();
        }

        public static IMqttClientOptions BuildOptions(MqttClientConfig mqttClientConfig)
        {
            var options = new MqttClientOptionsBuilder()
                .WithClientId(mqttClientConfig.ClientId)
                .WithTcpServer(mqttClientConfig.Broker, mqttClientConfig.Port)
                .WithCredentials(mqttClientConfig.UserName, mqttClientConfig.Password)
                .WithProtocolVersion(MqttProtocolVersion.V311)
                .WithCommunicationTimeout(TimeSpan.FromSeconds(10))
                .WithCleanSession();

            if (mqttClientConfig.UseSsl)
            {
                options = options.WithTls();
            }

            return options.Build();
        }
    }
}
