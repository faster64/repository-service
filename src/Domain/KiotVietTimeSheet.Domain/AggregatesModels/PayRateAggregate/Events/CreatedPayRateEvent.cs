using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Events
{
    public class CreatedPayRateEvent : DomainEvent
    {
        public PayRate PayRate { get; set; }

        public CreatedPayRateEvent(PayRate payRate)
        {
            PayRate = payRate;
        }
    }
}
