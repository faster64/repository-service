using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.Events
{
    public class SendRetailerInformationIntegrationEvent : IntegrationEvent
    {
        public ContactActiveFeatureReq Contact { get; set; }
        public SendRetailerInformationIntegrationEvent(ContactActiveFeatureReq contact)
        {
            Contact = contact;

        }
    }
}
