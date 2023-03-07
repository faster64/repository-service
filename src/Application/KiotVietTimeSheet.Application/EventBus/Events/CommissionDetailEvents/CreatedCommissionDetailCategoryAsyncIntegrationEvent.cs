using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents
{
    public class CreatedCommissionDetailCategoryAsyncIntegrationEvent : IntegrationEvent
    {
        public List<long> CommissionIds { get; set; }
        public List<int> CategoryIds { get; set; }
        
        [JsonConstructor]
        public CreatedCommissionDetailCategoryAsyncIntegrationEvent()
        {

        }

        public CreatedCommissionDetailCategoryAsyncIntegrationEvent(List<long> commissionIds, List<int> categoryIds)
        {
            CommissionIds = commissionIds;
            CategoryIds = categoryIds;
        }
    }
}
