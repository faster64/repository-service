using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.UpdateCommissionDetail
{
    public class UpdateCommissionDetailCommand : BaseCommand<List<CommissionDetailDto>>
    {
        public List<CommissionDetailDto> CommissionDetailDtoList { get; set; }

        public UpdateCommissionDetailCommand(List<CommissionDetailDto> commissionDetailDtoList)
        {
            CommissionDetailDtoList = commissionDetailDtoList;
        }
    }
}
