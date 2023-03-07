namespace KiotVietTimeSheet.Application.Dto
{
    public class ProductDetailDto
    {
        public decimal? Value { get; set; }
        public decimal? ValueRatio { get; set; }
        public long CommissionId { get; set; }
        public string CommissionName { get; set; }
    }
}
