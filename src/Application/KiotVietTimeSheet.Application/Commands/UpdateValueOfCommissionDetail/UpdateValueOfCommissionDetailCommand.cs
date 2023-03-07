using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Commands.UpdateValueOfCommissionDetail
{
    public class UpdateValueOfCommissionDetailCommand : BaseCommand<List<CommissionDetailDto>>
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

        public UpdateValueOfCommissionDetailCommand(ProductCommissionDetailDto product, List<long> totalCommissionIds, decimal? value,
                decimal? valueRatio, bool isUpdateForAllCommission, int categoryId, string productCodeKeyword,
                string productNameKeyword, CategoryCommissionDetailDto category)
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