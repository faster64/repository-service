using System;
using System.Net.Http;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Infrastructure.DbMaster;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace KiotVietTimeSheet.AuditTrailWorker.Infras.KiotVietInternalService
{
    public static class KiotVietAuditInternalServiceExtensions
    {
        public static void AddKiotVietInternalService(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IKiotVietInternalService, Impls.KiotVietInternalService>(sp =>
            {
                var masterDbSvc = sp.GetRequiredService<IMasterDbService>();
                var eventContextSvc = sp.GetRequiredService<IEventContextService>();
                var config = sp.GetRequiredService<IOptions<KiotVietInternalServiceConfiguration>>().Value;
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var client = httpClientFactory.CreateClient();

                SetHttpClientBaseInfo(client, config.BookingEndPoint, config.BookingInternalToken, eventContextSvc.Context.RetailerCode, eventContextSvc.Context.GroupId);
                
                return new Impls.KiotVietInternalService(client);
            });
        }

        private static void SetHttpClientBaseInfo(HttpClient httpClient, string endpoint, string internalToken, string retailerCode, int groupId)
        {
            httpClient.BaseAddress = new Uri(endpoint);
            httpClient.DefaultRequestHeaders.Add("Retailer", retailerCode);
            httpClient.DefaultRequestHeaders.Add("InternalApiToken", internalToken);
            httpClient.DefaultRequestHeaders.Add("X-GROUP-ID", groupId.ToString());
            httpClient.DefaultRequestHeaders.Add("X-RETAILER-CODE", retailerCode);
            httpClient.DefaultRequestHeaders.Add("App", "timesheet");
        }
    }
}
