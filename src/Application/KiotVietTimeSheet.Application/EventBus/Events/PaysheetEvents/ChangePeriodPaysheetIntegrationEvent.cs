using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents
{
    public class ChangePeriodPaysheetIntegrationEvent : IntegrationEvent
    {
        public long PaySheetId { get; set; }
        public DateTime? PaySheetCreateTimeOld { get; set; }
        public List<BranchDto> BranchesDto { get; set; }
        public PaysheetDto PaysheetOldDto { get; set; }

        public ChangePeriodPaysheetIntegrationEvent(long paySheetId, List<BranchDto> branchesDto, DateTime? paySheetCreateTimeOld, PaysheetDto paysheetOldDto)
        {
            PaySheetId = paySheetId;
            BranchesDto = branchesDto;
            PaySheetCreateTimeOld = paySheetCreateTimeOld;
            PaysheetOldDto = paysheetOldDto;
        }
    }
}
