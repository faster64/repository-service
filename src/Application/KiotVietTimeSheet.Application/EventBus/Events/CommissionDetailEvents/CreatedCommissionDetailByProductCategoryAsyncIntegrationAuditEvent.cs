using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents
{
    public class CreatedCommissionDetailByProductCategoryAsyncIntegrationAuditEvent : IntegrationEvent
    {
        public ProductCategoryReqDto ProductCategory { get; set; }
        public List<long> CommissionIds { get; set; }

        [JsonConstructor]
        public CreatedCommissionDetailByProductCategoryAsyncIntegrationAuditEvent()
        {

        }

        public CreatedCommissionDetailByProductCategoryAsyncIntegrationAuditEvent(List<long> commissionIds, ProductCategoryReqDto productCategory)
        {
            CommissionIds = commissionIds;
            ProductCategory = productCategory;
        }
    }
}
