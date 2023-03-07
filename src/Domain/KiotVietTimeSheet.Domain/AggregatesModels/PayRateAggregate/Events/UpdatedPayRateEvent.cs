using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Events
{
    public class UpdatedPayRateEvent : DomainEvent
    {
        public PayRate NewPayRate { get; }
        public PayRate OldPayRate { get; }
        public UpdatedPayRateEvent(PayRate oldPayRate, PayRate newPayRate)
        {
            NewPayRate = newPayRate;
            OldPayRate = oldPayRate;
        }
    }
}
