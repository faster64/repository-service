using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByProductCategory
{
    public class CreateCommissionDetailByProductCategoryCommand : BaseCommand<List<CommissionDetailDto>>
    {
        public List<long> CommissionIds { get; set; }
        public ProductCategoryReqDto ProductCategory { get; set; }
        public CreateCommissionDetailByProductCategoryCommand(List<long> commissionIds,
            ProductCategoryReqDto productCategory)
        {
            CommissionIds = commissionIds;
            ProductCategory = productCategory;
        }
    }
}
