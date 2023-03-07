using System;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.Commands.CreateTenantNationHoliday;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Common;
using KiotVietTimeSheet.BackgroundTasks.Configuration;
using KiotVietTimeSheet.BackgroundTasks.EventHandlers;
using KiotVietTimeSheet.BackgroundTasks.IntegrationEvents.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types
{
    public class ActiveTimesheetProcess
    {
        private readonly WorkerConfiguration _workerConfiguration;
        private readonly ILogger<ActivedFeatureIntegrationEventHandler> _logger;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly ICacheClient _cacheClient;
        private readonly Helper _helper = new Helper();

        public ActiveTimesheetProcess(
            WorkerConfiguration workerConfiguration,
            IMediator mediator,
            IConfiguration configuration,
            ICacheClient cacheClient,
            ILogger<ActivedFeatureIntegrationEventHandler> logger)        
        {
            _workerConfiguration = workerConfiguration;
            _logger = logger;
            _mediator = mediator;
            _configuration = configuration;
            _cacheClient = cacheClient;
        }

        private const string TimesheetFeatureKey = "TimeSheet";
        private const string TimesheetCrmServiceName = "Quản lý nhân viên";
        private const string CrmDateFormat = "yyyy-MM-dd";
        private const string CrmActiveFeatureActionType = "dich-vu-gia-tang";

        public async Task ActivedFeatureIntegration(ActivedFeatureIntegrationEvent @event)
        {
            if (@event.FeatureKey == TimesheetFeatureKey)
            {
                _logger.LogWarning($"Received event ActivedFeatureIntegrationEvent with eventId = {@event.Id}");
                try
                {
                    var kvCrmIntegrateEndpoint = _workerConfiguration.KvCrmIntegrateEndpoint;
                    if (string.IsNullOrEmpty(kvCrmIntegrateEndpoint))
                    {
                        var ex = new Exception("KvCrmIntegrateEnpoint not defined in config file");
                        throw ex;
                    }

                    using var client = new JsonHttpClient();
                    await client.PostAsync<object>(kvCrmIntegrateEndpoint, new
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
                    _logger.LogWarning("Send to Kv CRM system successfully");

                    if (@event.Context != null)
                        await StoreCacheHoliday(@event);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Send to Kv CRM system failed with error: {ex.Message}");
                }
            }

            if (@event.RetailerId > 0)
            {
                _cacheClient.Remove($"TimeSheet-worker:Retailers:{@event.RetailerId}");
            }
        }

        private async Task StoreCacheHoliday(ActivedFeatureIntegrationEvent @event)
        {
            var context = @event.Context;
            var nationalHolidayCreationMonthFromConfigure = _configuration.GetSection("NationalHolidayCreationMonth").Get<byte>();
            var nationalHolidayCreationMonth = 12;
            if (nationalHolidayCreationMonthFromConfigure > 0)
                nationalHolidayCreationMonth = nationalHolidayCreationMonthFromConfigure;

            var nationalHolidayCreationDayFromConfigure =
                _configuration.GetSection("NationalHolidayCreationDay").Get<byte>();

            var nationalHolidayCreationDay = 1;
            if (nationalHolidayCreationDayFromConfigure > 0)
                nationalHolidayCreationDay = nationalHolidayCreationDayFromConfigure;

            var holidays = await _mediator.Send(new CreateTenantNationHolidayCommand(context.TenantId, context.UserId,
                nationalHolidayCreationMonth, nationalHolidayCreationDay));

            if (holidays != null && holidays.Any())
                _helper.FlushCacheStore(_cacheClient, @event.Context.RetailerCode, nameof(Holiday));
        }

    }
}
