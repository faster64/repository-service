
namespace KiotVietTimeSheet.BackgroundTasks.Infras.Mqtt
{
    public class MqttClientPublishMessage
    {
        public string Topic { get; set; }

        public string Message { get; set; }

        public bool Retain { get; set; } = false;

        public int Qos { get; set; } = 0;
    }
}
