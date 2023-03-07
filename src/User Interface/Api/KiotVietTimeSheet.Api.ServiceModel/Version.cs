using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/version", "GET")]
    public class VersionReq : IReturn<object>
    {
    }
}