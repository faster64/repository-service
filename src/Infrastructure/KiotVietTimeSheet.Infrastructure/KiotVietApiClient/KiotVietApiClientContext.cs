namespace KiotVietTimeSheet.Infrastructure.KiotVietApiClient
{
    public class KiotVietApiClientContext
    {
        public string RetailerCode { get; set; }
        public int BranchId { get; set; }
        public string BearerToken { get; set; }
        public int? GroupId { get; set; }
        public int IndustryId { get; set; }
        public KvSystemContext Retail { get; set; }
        public KvSystemContext Fnb { get; set; }
        public KvSystemContext Booking { get; set; }

    }

    public class KvSystemContext
    {
        public string EndPoint { get; set; }
        public string InternalToken { get; set; }
        public int[] GroupIds { get; set; }
        public string KvVersion { get; set; }
    }
}
