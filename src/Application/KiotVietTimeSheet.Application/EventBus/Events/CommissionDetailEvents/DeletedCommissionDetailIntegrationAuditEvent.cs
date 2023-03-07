using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents
{
    public class DeletedCommissionDetailIntegrationAuditEvent : IntegrationEvent
    {
        public List<CommissionDetailDto> ListCommissionDetails { get; set; }
        public List<ProductCommissionDetailDto> Products { get; set; }

        [JsonConstructor]
        public DeletedCommissionDetailIntegrationAuditEvent()
        {

        }

        public DeletedCommissionDetailIntegrationAuditEvent(List<CommissionDetailDto> commissionDetails, List<ProductCommissionDetailDto> products)
        {
            ListCommissionDetails = commissionDetails;
            Products = products;
        }
    }
}
