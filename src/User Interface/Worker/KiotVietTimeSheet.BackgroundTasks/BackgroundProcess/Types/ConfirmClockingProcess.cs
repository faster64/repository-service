using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.EventBus.Events.ConfirmClockingEvents;
using KiotVietTimeSheet.Application.Queries.GetConfirmClockingsByBranchId;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Common;
using KiotVietTimeSheet.BackgroundTasks.Infras.Mqtt;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types
{
    public class ConfirmClockingProcess : BaseBackgroundProcess
    {
        private readonly bool _useMqtt;
        private readonly MqttClientWrapper _mqttClient;
        private readonly IMediator _mediator;

        public ConfirmClockingProcess(
            IConfiguration configuration,
            MqttClientWrapper mqttClient,
            IAuthService authService,
            IKiotVietInternalService kiotVietInternalService,
            IMediator mediator)
            : base(kiotVietInternalService, authService)
        {
            _useMqtt = bool.Parse(configuration.GetValue<string>("UseMqtt"));
            _mqttClient = mqttClient;
            _mediator = mediator;
        }


        public async Task CreateConfirmClockingAsync(CreatedConfirmClockingIntegrationEvent @event)
        {
            if (_useMqtt)
            {
                
                var result = await _mediator.Send(new GetConfirmClockingsByBranchIdQuery(@event.Context.BranchId));

                var json = JsonConvert.SerializeObject(new
                {
                    Message = $"CreateConfirmClocking",
                    NewConfirmClocking = result.Count,
                    EventType = EventTypeStatic.CreateConfirmClockingIntegrationSocket
                });
                var mqttMsg = new MqttClientPublishMessage
                {
                    Topic = "timesheet/CreateConfirmClockingTopic/" + @event.Context.TenantId + "/" + @event.Context.BranchId,
                    Message = JsonConvert.SerializeObject(json, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                    Qos = 0,
                    Retain = false
                };
                await _mqttClient.PublishAsync(mqttMsg);
            }
        }
    }

}