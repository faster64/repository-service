using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents
{
    public class CreatePaysheetProcessIntegrationEvent : IntegrationEvent
    {
        public long PaysheetId { get; set; }
        public string PaysheetCode { get; set; }
        public string PaysheetName { get; set; }
        public byte PaysheetStatus { get; set; }
        public string Message { get; set; }
        public string EventType { get; set; }
        public CreatePaysheetProcessIntegrationEvent(long paysheetId, string paysheetCode, string paysheetName, byte paysheetStatus)
        {
            PaysheetCode = paysheetCode;
            PaysheetName = paysheetName;
            PaysheetStatus = paysheetStatus;
            PaysheetId = paysheetId;
        }

        [JsonConstructor]
        public CreatePaysheetProcessIntegrationEvent()
        {

        }
    }
}
