using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByProduct
{
    public class CreateCommissionDetailByProductCommand : BaseCommand<List<CommissionDetailDto>>
    {
        public List<long> CommissionIds { get; set; }
        public ProductCommissionDetailDto Product { get; set; }

        public CreateCommissionDetailByProductCommand(List<long> commissionIds, ProductCommissionDetailDto product)
        {
            CommissionIds = commissionIds;
            Product = product;
        }
    }
}
