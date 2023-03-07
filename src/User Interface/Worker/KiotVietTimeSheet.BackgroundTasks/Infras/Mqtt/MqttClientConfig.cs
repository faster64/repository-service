using System;

namespace KiotVietTimeSheet.BackgroundTasks.Infras.Mqtt
{
    public class MqttClientConfig
    {
        public string ClientId { get; set; } = $"KiotVietTimeSheet_Mqtt_Bridge_{Guid.NewGuid()}";

        public string Broker { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool UseSsl { get; set; } = false;

        public int ReConnectPrediodInSecond { get; set; } = 3;
    }
}
