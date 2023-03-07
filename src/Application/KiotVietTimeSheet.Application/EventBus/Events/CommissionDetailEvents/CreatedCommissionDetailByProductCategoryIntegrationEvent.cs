using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents
{
    public class CreatedCommissionDetailByProductCategoryIntegrationEvent : IntegrationEvent
    {
        public List<CommissionDetailDto> ListCommissionDetails { get; set; }
        public ProductCategoryReqDto ProductCategory { get; set; }

        [JsonConstructor]
        public CreatedCommissionDetailByProductCategoryIntegrationEvent()
        {

        }

        public CreatedCommissionDetailByProductCategoryIntegrationEvent(List<CommissionDetailDto> commissionDetails, ProductCategoryReqDto productCategory)
        {
            ListCommissionDetails = commissionDetails;
            ProductCategory = productCategory;
        }
    }
}
