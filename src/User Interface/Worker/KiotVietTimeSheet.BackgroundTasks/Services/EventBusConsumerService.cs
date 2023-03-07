using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.AutoKeepingEvents;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents;
using KiotVietTimeSheet.Application.EventBus.Events.ConfirmClockingEvents;
using KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents;
using KiotVietTimeSheet.BackgroundTasks.Infras.Mqtt;
using KiotVietTimeSheet.BackgroundTasks.IntegrationEvents.Events;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace KiotVietTimeSheet.BackgroundTasks.Services
{
    public class EventBusConsumerService : BackgroundService
    {
        private readonly IEventBus _eventBus;
        private readonly MqttClientWrapper _clientWrapper;
        private readonly bool _useMqtt;

        public EventBusConsumerService(IEventBus eventBus, MqttClientWrapper clientWrapper, IConfiguration configuration)
        {
            _eventBus = eventBus;
            _clientWrapper = clientWrapper;
            _useMqtt = bool.Parse(configuration.GetValue<string>("UseMqtt"));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_useMqtt)
            {
                await _clientWrapper.StartAsync();
            }

            _eventBus.Subscribe<CreatedCommissionDetailByProductCategoryAsyncIntegrationEvent, IIntegrationEventHandler<CreatedCommissionDetailByProductCategoryAsyncIntegrationEvent>>();

            // PayslipsPayment
            _eventBus.Subscribe<CreatedPayslipPaymentIntegrationEvent, IIntegrationEventHandler<CreatedPayslipPaymentIntegrationEvent>>();
            _eventBus.Subscribe<VoidedPayslipPaymentIntegrationEvent, IIntegrationEventHandler<VoidedPayslipPaymentIntegrationEvent>>();

            _eventBus.Subscribe<CheckUpdateTenantNationalHolidayIntegrationEvent, IIntegrationEventHandler<CheckUpdateTenantNationalHolidayIntegrationEvent>>();
            // actived feature integration event
            _eventBus.Subscribe<ActivedFeatureIntegrationEvent, IIntegrationEventHandler<ActivedFeatureIntegrationEvent>>();
            _eventBus.Subscribe<SentMailIntegrationEvent, IIntegrationEventHandler<SentMailIntegrationEvent>>();
            _eventBus.Subscribe<CreatePaysheetEmptyIntegrationEvent, IIntegrationEventHandler<CreatePaysheetEmptyIntegrationEvent>>();
            _eventBus.Subscribe<AutoLoadingAndUpdatePaysheetIntegrationEvent, IIntegrationEventHandler<AutoLoadingAndUpdatePaysheetIntegrationEvent>>();
            _eventBus.Subscribe<ChangePeriodPaysheetIntegrationEvent, IIntegrationEventHandler<ChangePeriodPaysheetIntegrationEvent>>();
            _eventBus.Subscribe<CreateAutoGenerateClockingIntegrationEvent, IIntegrationEventHandler<CreateAutoGenerateClockingIntegrationEvent>>();
            _eventBus.Subscribe<UpdateClockingTimeIntegrationEvent, IIntegrationEventHandler<UpdateClockingTimeIntegrationEvent>>();
            _eventBus.Subscribe<AutoKeepingIntegrationEvent, IIntegrationEventHandler<AutoKeepingIntegrationEvent>>();
            _eventBus.Subscribe<CreatedConfirmClockingIntegrationEvent, IIntegrationEventHandler<CreatedConfirmClockingIntegrationEvent>>();
        }
    }
}
