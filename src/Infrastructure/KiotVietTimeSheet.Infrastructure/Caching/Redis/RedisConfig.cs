namespace KiotVietTimeSheet.Infrastructure.Caching.Redis
{
    public class RedisConfig
    {
        public string Servers { get; set; }
        public string SentinelMasterName { get; set; }
        public long DbNumber { get; set; }
        public string AuthPass { get; set; }
        public bool IsSentinel { get; set; }
        public bool IsStrictPool { get; set; }
        public int MaxPoolSize { get; set; }
        public int MaxPoolTimeout { get; set; }
        /// <summary>
        /// For ServiceStack Redis (by second)
        /// </summary>
        public int WaitBeforeForcingMasterFailover { get; set; }
    }
}
