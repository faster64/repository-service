using System;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.Events
{
    public class ActivedFeatureIntegrationEvent : IntegrationEvent
    {
        public int RetailerId { get; set; }
        public string FeatureKey { get; set; }
        public ActiveTypes ActiveType { get; set; }
        public DateTime? ActivedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public ContactActiveFeatureReq Contact { get; set; }

        public ActivedFeatureIntegrationEvent(int retailerId, string featureKey, ActiveTypes activeType, DateTime? activedDate, DateTime? expiredDate, ContactActiveFeatureReq contact)
        {
            RetailerId = retailerId;
            FeatureKey = featureKey;
            ActiveType = activeType;
            ActivedDate = activedDate;
            ExpiredDate = expiredDate;
            Contact = contact;
        }

        public enum ActiveTypes
        {
            Trial = 0,
            Paid = 1
        }
    }
}
