
using System.Threading.Tasks;
using KiotVietTimeSheet.SharedKernel.Auth;
using KiotVietTimeSheet.Utilities;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using ServiceStack;

namespace KiotVietTimeSheet.Api.Middlewares
{
    public class RequestLogContextMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLogContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            var session = HostContext.Resolve<ExecutionContext>();
            if (session.IsAuthorized())
            {
                using (LogContext.PushProperty("BuildVersion", Globals.BuildVersion))
                using (LogContext.PushProperty("Kv.RetailerCode", session.TenantCode))
                using (LogContext.PushProperty("Kv.BranchId", session.BranchId))
                using (LogContext.PushProperty("Kv.RetailerId", session.TenantId))
                using (LogContext.PushProperty("Kv.UserId", session.User.Id))
                using (LogContext.PushProperty("Kv.GroupId", session.User.GroupId))
                {
                    return _next.Invoke(context);
                }
            }

            using (LogContext.PushProperty("BuildVersion", Globals.BuildVersion))
            {
                return _next.Invoke(context);
            }
        }
    }
}
