namespace KiotVietTimeSheet.Infrastructure.DbMaster.Models
{
    public class KvGroup
    {
        public int Id { get; set; }

        public string ConnectionString { get; set; }

        public string PromotionConnectionString { get; set; }

        public bool IsCurrent { get; set; }

        public string Server { get; set; }

        public string Prefix { get; set; }

        public string Description { get; set; }

        public string TimeSheetConnectionString { get; set; }
    }
}
