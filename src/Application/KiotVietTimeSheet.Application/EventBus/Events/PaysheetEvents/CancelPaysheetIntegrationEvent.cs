using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents
{
    public class CancelPaysheetIntegrationEvent : IntegrationEvent
    {
        public Paysheet Paysheet { get; set; }
        public CancelPaysheetIntegrationEvent(CancelPaysheetEvent @event)
        {
            Paysheet = @event.Paysheet;
        }

        [JsonConstructor]
        public CancelPaysheetIntegrationEvent()
        {

        }
    }
}
