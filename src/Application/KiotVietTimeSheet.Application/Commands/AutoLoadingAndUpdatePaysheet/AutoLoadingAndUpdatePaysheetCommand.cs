using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;

namespace KiotVietTimeSheet.Application.Commands.AutoLoadingAndUpdatePaySheet
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Update)]
    public class AutoLoadingAndUpdatePaysheetCommand : BaseCommand<PaysheetDto>
    {
        public long PaySheetId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsAcceptLoading { get; set; }
        public List<BranchDto> BranchesDto { get; set; }

        public AutoLoadingAndUpdatePaysheetCommand(long paySheetId, DateTime modifiedDate, bool isAcceptLoading, List<BranchDto> branchesDto)
        {
            PaySheetId = paySheetId;
            ModifiedDate = modifiedDate;
            IsAcceptLoading = isAcceptLoading;
            BranchesDto = branchesDto;
        }
    }
}
