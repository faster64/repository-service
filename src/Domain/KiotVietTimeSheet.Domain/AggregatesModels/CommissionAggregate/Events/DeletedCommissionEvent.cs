using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Events
{
    public class DeletedCommissionEvent : DomainEvent
    {
        public Commission Commission { get; private set; }
        public DeletedCommissionEvent(Commission commission)
        {
            Commission = commission;
        }
    }
}
