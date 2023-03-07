using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Events
{
    public class CancelPaysheetEvent : DomainEvent
    {
        public Paysheet Paysheet { get; set; }

        public CancelPaysheetEvent(Paysheet paysheet)
        {
            Paysheet = paysheet;
        }
    }
}
