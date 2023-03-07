using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.PayrateEvents
{
    public class CreatedPayrateIntegrationEvent : IntegrationEvent
    {
        public PayRate PayRate { get; set; }

        public CreatedPayrateIntegrationEvent(PayRate payRate)
        {
            PayRate = payRate;
        }

        [JsonConstructor]
        public CreatedPayrateIntegrationEvent()
        {

        }
    }
}
