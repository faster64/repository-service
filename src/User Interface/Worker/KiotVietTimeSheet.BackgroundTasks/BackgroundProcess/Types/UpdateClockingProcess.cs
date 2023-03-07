using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Common;
using KiotVietTimeSheet.BackgroundTasks.Infras.Mqtt;
using KiotVietTimeSheet.Resources;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types
{
    public class UpdateClockingProcess : BaseBackgroundProcess
    {
        private readonly bool _useMqtt;
        private readonly MqttClientWrapper _mqttClient;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;

        public UpdateClockingProcess(
            IConfiguration configuration,
            MqttClientWrapper mqttClient,
            IAuthService authService,
            IKiotVietInternalService kiotVietInternalService,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository)
            : base(kiotVietInternalService, authService)
        {
            _useMqtt = bool.Parse(configuration.GetValue<string>("UseMqtt"));
            _mqttClient = mqttClient;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
        }

        public async Task UpdateClockingTimeAsync(UpdateClockingTimeIntegrationEvent @event)
        {
            var result = await _clockingWriteOnlyRepository.UpdateClockingTimeByShift(@event.TenantId, @event.BranchId, @event.ShiftId, @event.From, @event.To);
            if (_useMqtt)
            {
                var json = new
                {
                    @event.TenantId,
                    @event.BranchId,
                    Message = $"{string.Format(Message.updateClockingTime, result, @event.ShiftName)}",
                    EventType = EventTypeStatic.UpdateClockingTimeSocket
                };
                var mqttMsg = new MqttClientPublishMessage
                {
                    Topic = $"{MqttTopic.UpdateClockingTimeTopic}/{@event.TenantId}/{@event.BranchId}",
                    Message = JsonConvert.SerializeObject(json, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                    Qos = 0,
                    Retain = false
                };
                await _mqttClient.ForcePublishAsync(mqttMsg);
            }
        }
    }
}