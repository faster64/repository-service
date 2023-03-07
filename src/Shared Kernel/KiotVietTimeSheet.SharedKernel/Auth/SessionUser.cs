namespace KiotVietTimeSheet.SharedKernel.Auth
{
    public class SessionUser
    {
        public long Id { get; set; }
        public int RetailerId { get; set; }
        public byte Type { get; set; }
        public string UserName { get; set; }
        public string GivenName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public bool IsLimitTime { get; set; }
        public bool IsLimitedByTrans { get; set; }
        public bool IsShowSumRow { get; set; }
        public int GroupId { get; set; }
        public string KvSessionId { get; set; }
        public int IndustryId { get; set; }
        public string Language { get; set; }
        public SessionClockingGps SessionClockingGps { get; set; }
    }
}
