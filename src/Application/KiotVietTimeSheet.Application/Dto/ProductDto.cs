using System;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Dto
{
    public class ProductDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? Cost { get; set; }
        public decimal? LatestPurchasePrice { get; set; }
        public decimal? BasePrice { get; set; }
        public long CommissionId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<long> CommissionIds { get; set; }
        public ProductDetailDto Cd1 { get; set; }
        public ProductDetailDto Cd2 { get; set; }
        public ProductDetailDto Cd3 { get; set; }
        public ProductDetailDto Cd4 { get; set; }
        public ProductDetailDto Cd5 { get; set; }
    }
}
