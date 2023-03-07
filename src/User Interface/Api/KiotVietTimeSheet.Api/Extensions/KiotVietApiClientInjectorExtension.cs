using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Infrastructure.Configuration;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using KiotVietTimeSheet.Infrastructure.Securities.KvAuth;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Host;

namespace KiotVietTimeSheet.Api.Extensions
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

                var getFnbEndPoint = InfrastructureConfiguration.KiotVietServiceClientConfiguration.FnbEndPoint;
                var getFnbGroupIds = InfrastructureConfiguration.KiotVietServiceClientConfiguration.FnbGroupIds;
                var getFnbToken = InfrastructureConfiguration.KiotVietServiceClientConfiguration.FnBInternalToken;

                var getRetailEndPoint = InfrastructureConfiguration.KiotVietServiceClientConfiguration.RetailEndPoint;
                var getRetailerGroupIds = InfrastructureConfiguration.KiotVietServiceClientConfiguration.RetailGroupIds;
                var getRetailerToken = InfrastructureConfiguration.KiotVietServiceClientConfiguration.RetailerInternalToken;

                var request = HostContext.GetCurrentRequest();
                var authService = HostContext.ResolveService<AuthenticateService>(request);
                var accessToken = authService.Request.GetBearerToken();
                var getSession = authService.GetSession().ConvertTo<KVSession>();
                var kvVersion = InfrastructureConfiguration.KiotVietServiceClientConfiguration.KvVersion;

                return new KiotVietApiClientContext
                {
                    RetailerCode = getSession.CurrentRetailerCode,
                    BranchId = getSession.CurrentBranchId,
                    BearerToken = accessToken,
                    GroupId = getSession.GroupId,
                    IndustryId = getSession.CurrentIndustryId,
                    Retail = new KvSystemContext
                    {
                        EndPoint = getRetailEndPoint,
                        GroupIds = getRetailerGroupIds,
                        InternalToken = getRetailerToken
                    },
                    Fnb = new KvSystemContext
                    {
                        EndPoint = getFnbEndPoint,
                        GroupIds = getFnbGroupIds,
                        InternalToken = getFnbToken,
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
            services.AddScoped<IKiotVietApiClient, KiotVietApiClient>();
            services.AddScoped<IKiotVietServiceClient, KiotVietApiClient>();
        }
    }
}
