using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents
{
    public class DeletedCommissionDetailIntegrationEvent : IntegrationEvent
    {
        public List<CommissionDetailDto> ListCommissionDetails { get; set; }
        public List<ProductCommissionDetailDto> Products { get; set; }

        [JsonConstructor]
        public DeletedCommissionDetailIntegrationEvent()
        {

        }

        public DeletedCommissionDetailIntegrationEvent(List<CommissionDetailDto> commissionDetails, List<ProductCommissionDetailDto> products)
        {
            ListCommissionDetails = commissionDetails;
            Products = products;
        }
    }
}
