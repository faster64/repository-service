using System.Collections.Generic;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents
{
    public class CreatePaysheetEmptyIntegrationEvent : IntegrationEvent
    {
        public long PaySheetId { get; set; }
        public List<BranchDto> BranchesDto { get; set; }
        public CreatePaysheetEmptyIntegrationEvent(long paySheetId, List<BranchDto> branchesDto)
        {
            PaySheetId = paySheetId;
            BranchesDto = branchesDto;
        }
    }
}
