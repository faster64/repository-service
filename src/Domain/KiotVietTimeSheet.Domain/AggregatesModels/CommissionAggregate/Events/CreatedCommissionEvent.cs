using System.Collections.Generic;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Events
{
    public class CreatedCommissionEvent : DomainEvent
    {
        public Commission Commission { get; set; }
        public List<int> BranchIds { get; set; }

        public CreatedCommissionEvent(Commission commission, List<int> branchIds)
        {
            Commission = commission;
            BranchIds = branchIds;
        }
    }
}
