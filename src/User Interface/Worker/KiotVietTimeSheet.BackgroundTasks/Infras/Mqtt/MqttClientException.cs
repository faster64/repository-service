
using System;

namespace KiotVietTimeSheet.BackgroundTasks.Infras.Mqtt
{
    public class MqttClientException : Exception
    {
        public MqttClientException(string msg) : base(msg)
        {
        }
    }
}
