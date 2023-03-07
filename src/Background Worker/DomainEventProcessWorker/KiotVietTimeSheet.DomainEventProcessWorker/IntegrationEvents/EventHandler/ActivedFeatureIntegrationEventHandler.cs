using System;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Caching;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.DomainEventProcessWorker.Configuration;
using KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.Events;
using KiotVietTimeSheet.DomainEventProcessWorker.KvProcessFailEvent;
using KiotVietTimeSheet.DomainEventProcessWorker.Persistence;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Configuration;

namespace KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.EventHandler
{
    public class ActivedFeatureIntegrationEventHandler : IIntegrationEventHandler<ActivedFeatureIntegrationEvent>
    {
        private readonly IWorkerConfiguration _workerConfiguration;
        private readonly KvMasterDataDbContext _dbMaster;
        private EfDbContext _db;
        private readonly IKiotVietApiClient _kiotVietApiClient;
        private readonly IAppSettings _appSettings;
        private readonly ICacheClient _cacheClient;
        private ILogger<ActivedFeatureIntegrationEventHandler> _logger;
        private ILogger<ActivedFeatureIntegrationEventHandler> Logger => _logger ?? (_logger = HostContext.Resolve<ILogger<ActivedFeatureIntegrationEventHandler>>());
        private readonly Helper _helper = new Helper();
        public ActivedFeatureIntegrationEventHandler(IWorkerConfiguration workerConfiguration, KvMasterDataDbContext dbMaster, EfDbContext db, IKiotVietApiClient kiotVietApiClient, IAppSettings appSettings, ICacheClient cacheClient)
        {
            _workerConfiguration = workerConfiguration;
            _dbMaster = dbMaster;
            _db = db;
            _kiotVietApiClient = kiotVietApiClient;
            _appSettings = appSettings;
            _cacheClient = cacheClient;
        }

        private const string TimesheetFeatureKey = "TimeSheet";
        private const string TimesheetCrmServiceName = "Quản lý nhân viên";
        private const string CrmDateFormat = "yyyy-MM-dd";
        private const string CrmActiveFeatureActionType = "dich-vu-gia-tang";

        public async Task Handle(ActivedFeatureIntegrationEvent @event)
        {
            if (@event.FeatureKey == TimesheetFeatureKey && @event.ActiveType == ActivedFeatureIntegrationEvent.ActiveTypes.Trial)
            {
                Logger.LogWarning($"Received event ActivedFeatureIntegrationEvent with eventId = {@event.Id}");
                try
                {
                    if (string.IsNullOrEmpty(_workerConfiguration.KvCrmIntegrateEndpoint))
                    {
                        var ex = new Exception("KvCrmIntegrateEnpoint not defined in config file");
                        throw ex;
                    }

                    using (var client = new JsonHttpClient())
                    {
                        await client.PostAsync<object>(_workerConfiguration.KvCrmIntegrateEndpoint, new
                        {
                            act = CrmActiveFeatureActionType,
                            retailer_id = @event.RetailerId.ToString(),
                            ten_dich_vu = TimesheetCrmServiceName,
                            ngay_dang_ky_dung_thu = @event.ActivedDate?.ToString(CrmDateFormat) ?? string.Empty,
                            ngay_het_han = @event.ExpiredDate?.ToString(CrmDateFormat) ?? string.Empty,
                            ten_nguoi_lien_he = @event.Contact?.ContactName,
                            so_dien_thoai_lien_he = @event.Contact?.ContactPhone,
                            so_luong_nhan_vien = @event.Contact?.EmployeeNumber?.ToString()
                        });
                        Logger.LogWarning("Send to Kv CRM system successfully");
                    }

                    if (@event.Context != null)
                    {
                        using (_db = await _helper.GetDbContextByGroupId(@event.Context.GroupId,
                            @event.Context.RetailerCode, _cacheClient, _kiotVietApiClient, _appSettings))
                        {
                            var holidays = await _helper.CreateNationalHolidayAsync(@event.Context, _db, _appSettings);
                            if (holidays != null && holidays.Any())
                            {
                                await _db.SaveChangesAsync();
                                _helper.FlushCacheStore(_cacheClient, @event.Context.RetailerCode, nameof(Holiday));
                            }

                        }
                    }


                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Send to Kv CRM system failed with error: {ex.Message}");
                    await AddToRetryEventAsync(@event, ex);
                }
            }
        }

        private async Task AddToRetryEventAsync(ActivedFeatureIntegrationEvent @event, Exception ex)
        {
            var eventData = new RabbitPublishFailIntegrationEvent(@event.GetType().Name, JsonConvert.SerializeObject(@event, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            }));

            // Add to failed message table for retry
            _dbMaster.KvProcessFailEvent.Add(new KvProcessFailEvent.KvProcessFailEvent
            {
                EventId = @event.Id,
                EventType = nameof(RabbitPublishFailIntegrationEvent),
                State = (int)KvProcessFailEventStates.Pending,
                EventData = JsonConvert.SerializeObject(eventData, new JsonSerializerSettings
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
