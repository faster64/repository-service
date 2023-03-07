using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.DeleteCommissionDetail
{
    [RequiredPermission(TimeSheetPermission.TimeSheetShift_Delete)]
    public class DeleteCommissionDetailCommand : BaseCommand<List<CommissionDetailDto>>
    {
        public List<long> CommissionIds { get; set; }
        public List<ProductCommissionDetailDto> Products { get; set; }
        public List<long> CategoryIds { get; set; }

        public DeleteCommissionDetailCommand(List<long> commissionIds, List<ProductCommissionDetailDto> products, List<long> categoryIds)
        {
            CommissionIds = commissionIds;
            Products = products;
            CategoryIds = categoryIds;
        }
    }
}
