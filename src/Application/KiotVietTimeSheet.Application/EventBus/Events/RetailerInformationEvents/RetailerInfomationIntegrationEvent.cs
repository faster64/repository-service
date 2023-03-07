using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.RetailerInformationEvents
{
    public class RetailerInformationIntegrationEvent : IntegrationEvent
    {
        public ContactActiveFeatureReq ContactFeatureReq { get; set; }

        public RetailerInformationIntegrationEvent(ContactActiveFeatureReq @event)
        {
            ContactFeatureReq = @event;
        }

        [JsonConstructor]
        public RetailerInformationIntegrationEvent()
        {

        }
    }
}
