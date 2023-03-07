using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Events
{
    public class DeletedTimeSheetEvent : DomainEvent
    {
        public long Id { get; set; }
    }
}
