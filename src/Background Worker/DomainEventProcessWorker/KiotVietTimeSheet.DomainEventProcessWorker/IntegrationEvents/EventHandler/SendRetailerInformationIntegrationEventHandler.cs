using System;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.RetailerInformationEvents;
using KiotVietTimeSheet.DomainEventProcessWorker.Configuration;
using KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.Events;
using KiotVietTimeSheet.DomainEventProcessWorker.KvProcessFailEvent;
using KiotVietTimeSheet.DomainEventProcessWorker.Persistence;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceStack;

namespace KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.EventHandler
{
    public class SendRetailerInformationIntegrationEventHandler : IIntegrationEventHandler<RetailerInformationIntegrationEvent>
    {
        private readonly IWorkerConfiguration _workerConfiguration;
        private readonly KvMasterDataDbContext _dbMaster;
        private readonly ILogger<SendRetailerInformationIntegrationEventHandler> _logger;

        public SendRetailerInformationIntegrationEventHandler(
            IWorkerConfiguration workerConfiguration, 
            KvMasterDataDbContext dbMaster, 
            ILogger<SendRetailerInformationIntegrationEventHandler> logger
        )
        {
            _workerConfiguration = workerConfiguration;
            _dbMaster = dbMaster;
            _logger = logger;
        }

        private const string CrmActiveFeatureActionType = "dich-vu-gia-tang";
        private const string CrmServiceName = "Quản lý nhân viên";
        public async Task Handle(RetailerInformationIntegrationEvent @event)
        {
            _logger.LogInformation($"Received event sendRetailerInformation with eventId = {@event.Id}");
            try
            {
                if (string.IsNullOrEmpty(_workerConfiguration.KvCrmIntegrateEndpoint))
                {
                    var ex = new Exception("KvCrmIntegrateEndpoint not defined in config file");
                    throw ex;
                }
                using (var client = new JsonHttpClient())
                {
                    await client.PostAsync<object>(_workerConfiguration.KvCrmIntegrateEndpoint, new
                    {
                        act = CrmActiveFeatureActionType,
                        retailer_id = @event.Context.TenantId.ToString(),
                        ten_dich_vu = CrmServiceName,
                        ten_nguoi_lien_he = @event.ContactFeatureReq?.ContactName,
                        so_dien_thoai_lien_he = @event.ContactFeatureReq?.ContactPhone,
                        so_luong_nhan_vien = @event.ContactFeatureReq?.EmployeeNumber?.ToString(),
                        ghi_chu = _workerConfiguration.KvCrmNote
                    });
                     _logger.LogInformation("Send to Kv CRM system successfully");
                }
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, $"Send to Kv CRM system failed with error: {ex.Message}");
                await AddToRetryEventAsync(@event, ex);
            }

        }

        private async Task AddToRetryEventAsync(RetailerInformationIntegrationEvent @event, Exception ex)
        {
            var retailerInfoEventData = new RabbitPublishFailIntegrationEvent(@event.GetType().Name, JsonConvert.SerializeObject(@event, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            }));

            // Add to failed message table for retry
            _dbMaster.KvProcessFailEvent.Add(new KvProcessFailEvent.KvProcessFailEvent
            {
                EventId = @event.Id,
                EventType = nameof(RabbitPublishFailIntegrationEvent),
                State = (int)KvProcessFailEventStates.Pending,
                EventData = JsonConvert.SerializeObject(retailerInfoEventData, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                }),
                ErrorMessage = ex.Message,
                RetryTimes = 0,
                CreatedTime = DateTime.Now,
            });
            await _dbMaster.SaveChangesAsync();
        }
    }
}
