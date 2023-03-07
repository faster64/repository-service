using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Events
{
    public class CreatedPaysheetEvent : DomainEvent
    {
        public Paysheet Paysheet { get; }
        public CreatedPaysheetEvent(Paysheet paysheet)
        {
            Paysheet = paysheet;
        }
    }
}
