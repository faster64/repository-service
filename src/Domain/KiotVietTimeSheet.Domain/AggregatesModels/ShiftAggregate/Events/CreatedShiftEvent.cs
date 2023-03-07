using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Events
{
    public class CreatedShiftEvent : DomainEvent
    {
        public Shift Shift { get; private set; }
        public bool IsGeneralSetting { get; private set; }
        public CreatedShiftEvent(Shift shift, bool isGeneralSetting = false)
        {
            Shift = shift;
            IsGeneralSetting = isGeneralSetting;
        }
    }
}
