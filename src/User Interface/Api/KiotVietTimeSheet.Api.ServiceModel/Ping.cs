using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/ping", "GET", Summary = "Ping API",
    Notes = "Kiểm tra service đang hoạt động hay không")]
    public class PingReq : IReturn<string>
    {

    }
}
