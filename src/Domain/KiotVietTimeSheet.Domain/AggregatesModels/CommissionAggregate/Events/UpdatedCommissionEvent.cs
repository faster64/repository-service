using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Events
{
    public class UpdatedCommissionEvent : DomainEvent
    {
        public Commission Commission { get; set; }
        public List<int> BranchIds { get; set; }

        public UpdatedCommissionEvent(Commission commission, List<int> branchIds)
        {
            Commission = commission;
            BranchIds = branchIds;
        }
    }
}
