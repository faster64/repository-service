namespace KiotVietTimeSheet.Infrastructure.EventBus.RabbitMQ
{
    public class RabbitMqEventBusConfiguration
    {
        public string Connection { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int? RetryCount { get; set; }
        public string QueueName { get; set; }
        public string VirtualHost { get; set; }
        public ushort? RequestedHeartbeat { get; set; }
        public bool UseSsL { get; set; }
        public bool AutomaticRecoveryEnabled { get; set; }
    }
}
