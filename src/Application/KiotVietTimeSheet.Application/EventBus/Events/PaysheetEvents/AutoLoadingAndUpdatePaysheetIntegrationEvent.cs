using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.SharedKernel.EventBus;
namespace KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents
{
    public class AutoLoadingAndUpdatePaysheetIntegrationEvent : IntegrationEvent
    {
        public long PaySheetId { get; set; }
        public int? StandardWorkingDayNumber { get; set; }
        public int? TimeOfStandardWorkingDay { get; set; }
        public DateTime? PaySheetCreateTimeOld { get; set; }
        public List<BranchDto> BranchesDto { get; set; }

        public AutoLoadingAndUpdatePaysheetIntegrationEvent(long paySheetId, int? standardWorkingDayNumber, int? timeOfStandardWorkingDay, List<BranchDto> branchesDto, DateTime? paySheetCreateTimeOld)
        {
            PaySheetId = paySheetId;
            StandardWorkingDayNumber = standardWorkingDayNumber;
            TimeOfStandardWorkingDay = timeOfStandardWorkingDay;
            BranchesDto = branchesDto;
            PaySheetCreateTimeOld = paySheetCreateTimeOld;
        }
    }
}
