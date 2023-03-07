using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Events
{
    public class UpdatedPaysheetEvent : DomainEvent
    {
        public Paysheet OldPaysheet { get; }
        public Paysheet NewPaysheet { get; }
        public UpdatedPaysheetEvent(Paysheet oldPaysheet, Paysheet newPaysheet)
        {
            OldPaysheet = oldPaysheet;
            NewPaysheet = newPaysheet;
        }
    }
}
