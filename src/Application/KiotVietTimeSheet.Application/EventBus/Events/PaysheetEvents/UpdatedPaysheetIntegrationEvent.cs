using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents
{
    public class UpdatedPaysheetIntegrationEvent : IntegrationEvent
    {
        public Paysheet OldPaysheet { get; set; }
        public Paysheet NewPaysheet { get; set; }
        public UpdatedPaysheetIntegrationEvent(Paysheet oldPaysheet, Paysheet newPaysheet)
        {
            OldPaysheet = oldPaysheet;
            NewPaysheet = newPaysheet;
        }

        [JsonConstructor]
        public UpdatedPaysheetIntegrationEvent()
        {

        }
    }
}
