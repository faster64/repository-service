using System;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.UpdatePaysheetTemporaryStatus
{
    public class UpdatePaysheetTemporaryStatusCommand : BaseCommand<PaysheetDto>
    {
        public long PaysheetId { get; set; }
        public DateTime? PaySheetCreateTimeOld { get; set; }
        public PaysheetDto PaysheetOldDto { get; set; }
        public int PaySheetError { get; set; }

        public UpdatePaysheetTemporaryStatusCommand(long paysheetId, DateTime? paySheetCreateTimeOld, PaysheetDto paysheetOldDto, int paySheetError)
        {
            PaysheetId = paysheetId;
            PaySheetCreateTimeOld = paySheetCreateTimeOld;
            PaysheetOldDto = paysheetOldDto;
            PaySheetError = paySheetError;
        }
    }
}
