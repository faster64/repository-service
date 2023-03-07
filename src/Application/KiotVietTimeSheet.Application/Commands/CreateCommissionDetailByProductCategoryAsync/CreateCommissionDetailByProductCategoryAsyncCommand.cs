using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByProductCategoryAsync
{
    public class CreateCommissionDetailByProductCategoryAsyncCommand : BaseCommand<Unit>
    {
        public List<long> CommissionIds { get; set; }
        public ProductCategoryReqDto ProductCategory { get; set; }

        public CreateCommissionDetailByProductCategoryAsyncCommand(List<long> commissionIds,
            ProductCategoryReqDto productCategory)
        {
            CommissionIds = commissionIds;
            ProductCategory = productCategory;
        }
    }
}
