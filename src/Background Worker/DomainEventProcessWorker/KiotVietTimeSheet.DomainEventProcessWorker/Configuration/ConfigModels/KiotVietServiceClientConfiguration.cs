namespace KiotVietTimeSheet.DomainEventProcessWorker.Configuration.ConfigModels
{
    public class KiotVietServiceClientConfiguration
    {
        public string FnbEndPoint { get; set; }
        public int[] FnbGroupIds { get; set; }
        public string RetailEndPoint { get; set; }
        public int[] RetailGroupIds { get; set; }
        public string RetailerInternalToken { get; set; }
        public string FnBInternalToken { get; set; }
        public string KvVersion { get; set; }
    }
}
