using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Utilities;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class VersionApi : Service
    {
        public object Get(VersionReq req)
        {
            return Globals.BuildVersion;
        }
    }
}
