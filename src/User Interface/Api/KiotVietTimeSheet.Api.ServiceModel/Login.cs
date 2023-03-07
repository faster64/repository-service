using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    public class Login
    {
        [Route("/login-clocking-gps", "POST")]
        public class LoginForClockingGpsReq
        {
            public int TenantId { get; set; }
            public string TenantCode { get; set; }
            public string QrKey { get; set; }
        }
    }
}
