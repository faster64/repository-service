using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents
{
    public class UpdatePaysheetExistedIntergrationEvent : IntegrationEvent
    {
        public PaysheetDto PaysheetDto { get; set; }

        public UpdatePaysheetExistedIntergrationEvent(PaysheetDto paysheetDto)
        {
            PaysheetDto = paysheetDto;
        }
    }
}
