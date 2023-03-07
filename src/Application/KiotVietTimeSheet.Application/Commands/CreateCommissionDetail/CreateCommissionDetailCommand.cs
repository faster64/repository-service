using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Commands.CreateCommissionDetail
{
    public class CreateCommissionDetailCommand : BaseCommand<List<CommissionDetailDto>>
    {
        public List<CommissionDetailDto> CommissionDetailDtoList { get; set; }

        public bool IsNotAudit { get; set; }

        public CreateCommissionDetailCommand(List<CommissionDetailDto> commissionDetailDtoList)
        {
            CommissionDetailDtoList = commissionDetailDtoList;
        }
    }
}
