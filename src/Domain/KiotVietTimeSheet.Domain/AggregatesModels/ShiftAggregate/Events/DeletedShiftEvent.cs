using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Events
{
    public class DeletedShiftEvent : DomainEvent
    {
        public Shift Shift { get; private set; }
        public bool IsGeneralSetting { get; set; }
        public DeletedShiftEvent(Shift shift, bool isGeneralSetting)
        {
            Shift = shift;
            IsGeneralSetting = isGeneralSetting;
        }
    }
}
