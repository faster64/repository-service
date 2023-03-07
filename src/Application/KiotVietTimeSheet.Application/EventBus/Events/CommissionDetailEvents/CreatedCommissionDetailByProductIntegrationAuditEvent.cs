using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents
{
    public class CreatedCommissionDetailByProductIntegrationAuditEvent : IntegrationEvent
    {
        public List<CommissionDetailDto> ListCommissionDetails { get; set; }
        public ProductCommissionDetailDto Product { get; set; }

        [JsonConstructor]
        public CreatedCommissionDetailByProductIntegrationAuditEvent()
        {

        }

        public CreatedCommissionDetailByProductIntegrationAuditEvent(List<CommissionDetailDto> commissionDetails, ProductCommissionDetailDto product)
        {
            ListCommissionDetails = commissionDetails;
            Product = product;
        }
    }
}
