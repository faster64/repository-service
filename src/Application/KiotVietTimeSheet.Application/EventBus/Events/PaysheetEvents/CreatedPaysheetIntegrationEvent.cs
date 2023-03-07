using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;

namespace KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents
{
    public class CreatedPaysheetIntegrationEvent : IntegrationEvent
    {
        public Paysheet Paysheet { get; set; }
        public CreatedPaysheetIntegrationEvent(Paysheet paysheet)
        {
            Paysheet = paysheet;
        }

        [JsonConstructor]
        public CreatedPaysheetIntegrationEvent()
        {

        }
    }
}
