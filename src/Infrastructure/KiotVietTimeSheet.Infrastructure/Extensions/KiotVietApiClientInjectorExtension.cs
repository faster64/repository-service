using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Infrastructure.Configuration;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using KiotVietTimeSheet.Infrastructure.Securities.KvAuth;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Host;

namespace KiotVietTimeSheet.Infrastructure.Extensions
{
    public static class KiotVietApiClientInjectorExtension
    {
        public static void AddKiotVietApiClient(this IServiceCollection services)
        {
            services.AddScoped(sp =>
            {
                var bookingEndPoint = InfrastructureConfiguration.KiotVietServiceClientConfiguration.BookingEndPoint;
                var bookingGroupIds = InfrastructureConfiguration.KiotVietServiceClientConfiguration.BookingGroupIds;
                var bookingToken = InfrastructureConfiguration.KiotVietServiceClientConfiguration.BookingInternalToken;

                var fnbEndPoint = InfrastructureConfiguration.KiotVietServiceClientConfiguration.FnbEndPoint;
                var fnbGroupIds = InfrastructureConfiguration.KiotVietServiceClientConfiguration.FnbGroupIds;
                var fnbToken = InfrastructureConfiguration.KiotVietServiceClientConfiguration.FnBInternalToken;

                var retailerGroupIds = InfrastructureConfiguration.KiotVietServiceClientConfiguration.RetailGroupIds;
                var retailEndPoint = InfrastructureConfiguration.KiotVietServiceClientConfiguration.RetailEndPoint;
                var retailerToken = InfrastructureConfiguration.KiotVietServiceClientConfiguration.RetailerInternalToken;

                var request = HostContext.GetCurrentRequest();
                var authService = HostContext.ResolveService<AuthenticateService>(request);
                var accessToken = authService.Request.GetBearerToken();
                var session = authService.GetSession().ConvertTo<KVSession>();
                var kvVersion = InfrastructureConfiguration.KiotVietServiceClientConfiguration.KvVersion;

                return new KiotVietApiClientContext
                {
                    RetailerCode = session.CurrentRetailerCode,
                    BranchId = session.CurrentBranchId,
                    BearerToken = accessToken,
                    GroupId = session.GroupId,
                    IndustryId = session.CurrentIndustryId,
                    Retail = new KvSystemContext
                    {
                        EndPoint = retailEndPoint,
                        GroupIds = retailerGroupIds,
                        InternalToken = retailerToken
                    },
                    Fnb = new KvSystemContext
                    {
                        EndPoint = fnbEndPoint,
                        GroupIds = fnbGroupIds,
                        InternalToken = fnbToken,
                        KvVersion = kvVersion
                    },
                    Booking = new KvSystemContext
                    {
                        EndPoint = bookingEndPoint,
                        GroupIds = bookingGroupIds,
                        InternalToken = bookingToken,
                        KvVersion = kvVersion
                    }
                };
            });
            services.AddScoped<IKiotVietApiClient, KiotVietApiClient.KiotVietApiClient>();
            services.AddScoped<IKiotVietServiceClient, KiotVietApiClient.KiotVietApiClient>();
        }
    }
}