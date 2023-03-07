using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;

namespace KiotVietTimeSheet.Application.Commands.PaysheetWhenChangeWorkingPeriod
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Read)]
    public class PaysheetWhenChangeWorkingPeriodCommand : BaseCommand<PaysheetDto>
    {
        public PaysheetDto PaysheetDto { get; set; }
        public List<BranchDto> BranchesDto { get; set; }

        public PaysheetWhenChangeWorkingPeriodCommand(PaysheetDto paysheetDto, List<BranchDto> branchesDto)
        {
            PaysheetDto = paysheetDto;
            BranchesDto = branchesDto;
        }
    }
}
