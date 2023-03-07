namespace KiotVietTimeSheet.Application.Dto
{
    public class AllowanceDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int TenantId { get; set; }
        public string Code { get; set; }
        public bool IsDeleted { get; set; }
        public bool? SelectedItem { get; set; }
        public double Value { get; set; }
        public double ValueRatio { get; set; }
        public bool IsChecked { get; set; }
        public int? Type { get; set; }
        public double Rank { get; set; }
    }
}
