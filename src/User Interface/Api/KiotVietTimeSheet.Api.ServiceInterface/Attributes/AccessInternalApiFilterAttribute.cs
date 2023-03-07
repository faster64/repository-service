using KiotVietTimeSheet.Infrastructure.Configuration;
using ServiceStack;
using ServiceStack.Web;
using System.Net;
using Network = KiotVietTimeSheet.Utilities.Network;

namespace KiotViet.Web.Api.Filters
{
    public class AccessInternalApiFilterAttribute : RequestFilterAttribute
    {
        public override void Execute(IRequest req, IResponse res, object requestDto)
        {
            var ip = req.RemoteIp;
            var whiteList = InfrastructureConfiguration.KiotVietServiceClientConfiguration.InternalIpWhitelist;
            var bookingToken = InfrastructureConfiguration.KiotVietServiceClientConfiguration.TimeSheetBookingInternalToken;
            if (!Network.IsIpAddressValid(ip, whiteList))
            {
                throw new HttpError(HttpStatusCode.Forbidden, "You don't have permission to access on this server");
            }
            // check secret key from header of request
            if (!string.IsNullOrEmpty(bookingToken) &&
                req.Headers["InternalApiKey"] != bookingToken)
                throw HttpError.Unauthorized("You don't have permission to access on this api");
        }
    }
}
