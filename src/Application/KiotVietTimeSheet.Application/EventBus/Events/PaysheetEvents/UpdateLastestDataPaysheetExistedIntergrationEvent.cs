using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents
{
    public class UpdateLastestDataPaysheetExistedIntergrationEvent : IntegrationEvent
    {
        public long PaySheetId { get; set; }

        public UpdateLastestDataPaysheetExistedIntergrationEvent(long paySheetId)
        {
            PaySheetId = paySheetId;
        }
    }
}
