using System;
using System.Linq;
using System.Net.Http;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Infrastructure.Configuration;
using KiotVietTimeSheet.Infrastructure.DbMaster;
using KiotVietTimeSheet.Infrastructure.KiotVietInternalServices;
using KiotVietTimeSheet.Infrastructure.Securities.KvAuth;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using ServiceStack.Auth;

namespace KiotVietTimeSheet.Infrastructure.Extensions
{
    public static class KiotVietInternalServiceExtensions
    {
        public static void AddInternalService(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IKiotVietInternalService, KiotVietInternalService>(sp =>
            {
                var request = HostContext.GetCurrentRequest();
                var authService = HostContext.ResolveService<AuthenticateService>(request);
                var session = authService.GetSession().ConvertTo<KVSession>();
                
                var bookingEndPoint = InfrastructureConfiguration.KiotVietServiceClientConfiguration.BookingEndPoint;
                var bookingToken = InfrastructureConfiguration.KiotVietServiceClientConfiguration.BookingInternalToken;

                var fnbEndPoint = InfrastructureConfiguration.KiotVietServiceClientConfiguration.FnbEndPoint;
                var fnBInternalToken = InfrastructureConfiguration.KiotVietServiceClientConfiguration.FnBInternalToken;

                var retailerEndPoint = InfrastructureConfiguration.KiotVietServiceClientConfiguration.RetailEndPoint;
                var retailerInternalToken = InfrastructureConfiguration.KiotVietServiceClientConfiguration.RetailerInternalToken;

                var masterDbContext = sp.GetRequiredService<DbMasterContext>();
                var retailer = masterDbContext?.KvRetailer.FirstOrDefault(x => x.Code == session.CurrentRetailerCode);
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var client = httpClientFactory.CreateClient();

                if (retailer != null && retailer.IsFnB())
                {
                    SetHttpClientBaseInfo(client, fnbEndPoint, fnBInternalToken, session.CurrentRetailerCode, session.GroupId);
                }
                else if (retailer == null || retailer.IsBooking())
                {
                    SetHttpClientBaseInfo(client, bookingEndPoint, bookingToken, session.CurrentRetailerCode, session.GroupId);
                }
                else
                {
                    SetHttpClientBaseInfo(client, retailerEndPoint, retailerInternalToken, session.CurrentRetailerCode, session.GroupId);
                }
                return new KiotVietInternalService(client);
            });
        }

        private static void SetHttpClientBaseInfo(HttpClient httpClient, string endpoint, string internalToken, string retailerCode, int groupId)
        {
            httpClient.BaseAddress = new Uri(endpoint);
            httpClient.DefaultRequestHeaders.Add("InternalApiToken", internalToken);
            httpClient.DefaultRequestHeaders.Add("Retailer", retailerCode);
            httpClient.DefaultRequestHeaders.Add("X-GROUP-ID", groupId.ToString());
            httpClient.DefaultRequestHeaders.Add("X-RETAILER-CODE", retailerCode);
            httpClient.DefaultRequestHeaders.Add("App", "timesheet");
        }
    }
}
