using KiotVietTimeSheet.Infrastructure.Configuration;
using KiotVietTimeSheet.Infrastructure.Securities.KvAuth;
using KiotVietTimeSheet.SharedKernel.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Host;

namespace KiotVietTimeSheet.Api.Extensions
{
    public static class ExecutionContextInjectorExtension
    {
        public static void AddExecutionContext(this IServiceCollection services)
        {
            services.AddScoped(c =>
            {
                var request = HostContext.GetCurrentRequest();
                if (request.Headers != null && !string.IsNullOrEmpty(request.Headers["InternalApiKey"]))
                {
                    if (!string.IsNullOrEmpty(request.Headers["RetailerId"]) && !string.IsNullOrEmpty(request.Headers["UserId"]))
                    {
                        return new ExecutionContext()
                        {
                            TenantId = int.Parse(request.Headers["RetailerId"]),
                            TenantCode = request.Headers["Retailer"],
                            //BranchId = int.Parse(request.Headers["BranchId"])
                            User = new SessionUser()
                            {
                                Id = long.Parse(request.Headers["UserId"]),
                                IsAdmin = true
                            }
                        };
                    }

                    return new ExecutionContext();
                }
                var authService = HostContext.ResolveService<AuthenticateService>(request);
                var session = authService.GetSession().ConvertTo<KVSession>();
                var configuration = c.GetRequiredService<IConfiguration>();//NOSONAR
                var context = new ExecutionContext
                {
                    User = session.CurrentUser,
                    TenantId = session.CurrentRetailerId,
                    TenantCode = session.CurrentRetailerCode,
                    BranchId = session.CurrentBranchId,
                    JwtToken = request.GetBearerToken(),
                    HttpMethod = request.Verb,
                    Language = session.Language ?? "vi-VN"
                };

                var sessionInfo = session.SessionInfo;

                if (sessionInfo == null) return context;

                context.TimeSheetConnection = InfrastructureConfiguration.ConnectionString;
                if (string.IsNullOrEmpty(sessionInfo.TimeSheetConnectionString)) return context;
                var shardingConnection = configuration.GetConnectionString(sessionInfo.TimeSheetConnectionString);
                if (!string.IsNullOrEmpty(shardingConnection)) context.TimeSheetConnection = shardingConnection;
                return context;
            });
        }
    }
}
