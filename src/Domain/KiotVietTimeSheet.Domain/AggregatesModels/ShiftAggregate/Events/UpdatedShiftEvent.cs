using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Events
{
    public class UpdatedShiftEvent : DomainEvent
    {
        public Shift OldShift { get; set; }
        public Shift Shift { get; set; }
        public bool IsGeneralSetting { get; set; }

        public UpdatedShiftEvent(Shift oldShift, Shift shift, bool isGeneralSetting)
        {
            OldShift = oldShift;
            Shift = shift;
            IsGeneralSetting = isGeneralSetting;
        }
    }
}
