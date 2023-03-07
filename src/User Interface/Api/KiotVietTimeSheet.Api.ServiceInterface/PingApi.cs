using ServiceStack;
using KiotVietTimeSheet.Api.ServiceModel;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class PingApi : Service
    {
        public PingApi()
        {
        }

        public string Get(PingReq req)
        {
            return "pong";
        }
    }
}
