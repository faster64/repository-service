using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateMultipleCommissionDetail
{
    public class CreateMultipleCommissionDetailCommand : BaseCommand<List<CommissionDetailDto>>
    {
        public ProductCommissionDetailDto Product { get; set; }
        public List<long> TotalCommissionIds { get; set; }
        public decimal? Value { get; set; }
        public decimal? ValueRatio { get; set; }
        public bool IsUpdateForAllCommission { get; set; }
        public int CategoryId { get; set; }
        public string ProductCodeKeyword { get; set; }
        public string ProductNameKeyword { get; set; }
        public CategoryCommissionDetailDto Category { get; set; }

        public CreateMultipleCommissionDetailCommand(ProductCommissionDetailDto product, List<long> totalCommissionIds,
            decimal? value, decimal? valueRatio, bool isUpdateForAllCommission, int categoryId,
            string productCodeKeyword, string productNameKeyword, CategoryCommissionDetailDto category)
        {
            Product = product;
            TotalCommissionIds = totalCommissionIds;
            Value = value;
            ValueRatio = valueRatio;
            IsUpdateForAllCommission = isUpdateForAllCommission;
            CategoryId = categoryId;
            ProductCodeKeyword = productCodeKeyword;
            ProductNameKeyword = productNameKeyword;
            Category = category;
        }
    }
}
