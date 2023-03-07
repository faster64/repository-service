using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents
{
    public class CreatedCommissionDetailByProductCategoryAsyncIntegrationEvent : IntegrationEvent
    {
        public ProductCategoryReqDto ProductCategory { get; set; }
        public List<long> CommissionIds { get; set; }

        [JsonConstructor]
        public CreatedCommissionDetailByProductCategoryAsyncIntegrationEvent()
        {

        }

        public CreatedCommissionDetailByProductCategoryAsyncIntegrationEvent(List<long> commissionIds, ProductCategoryReqDto productCategory)
        {
            CommissionIds = commissionIds;
            ProductCategory = productCategory;
        }
    }
}
