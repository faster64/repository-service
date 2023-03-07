using System.Net;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Web;

namespace KiotVietTimeSheet.Api.ServiceInterface.Attributes
{
    public class InternalAccessAttribute : RequestFilterAttribute
    {
        public override void Execute(IRequest req, IResponse res, object requestDto)
        {
            var apiKeySettings = HostContext.TryResolve<IAppSettings>()?.GetString("InternalApiKey");
            if (req == null)
            {
                throw new HttpError(HttpStatusCode.Unauthorized, "You dont't have permission to access on this api");
            }

            if (string.IsNullOrEmpty(req.Headers["InternalApiKey"]) || string.IsNullOrEmpty(apiKeySettings))
            {
                throw new HttpError(HttpStatusCode.Unauthorized, "You dont't have permission to access on this api");
            }

            if (!string.IsNullOrEmpty(req.Headers["InternalApiKey"]) &&
                !req.Headers["InternalApiKey"].Equals(apiKeySettings))
            {
                throw new HttpError(HttpStatusCode.Unauthorized, "You dont't have permission to access on this api");
            }
        }
    }
}