using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.Commands.CreateTenantNationHoliday;
using KiotVietTimeSheet.BackgroundTasks.IntegrationEvents.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Common;

namespace KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types
{
    public class HolidayProcess : BaseBackgroundProcess
    {
        private readonly ILogger<HolidayProcess> _logger;
        private readonly ICacheClient _cacheClient;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly Helper _helper = new Helper();

        public HolidayProcess(
            IKiotVietInternalService kiotVietInternalService,
            IMediator mediator,
            IConfiguration configuration,
            ICacheClient cacheClient,
            IAuthService authService,
            ILogger<HolidayProcess> logger) : base(kiotVietInternalService, authService)
        {
            _mediator = mediator;
            _configuration = configuration;
            _logger = logger;
            _cacheClient = cacheClient;
        }

        public async Task CheckUpdateTenantNationHoliday(CheckUpdateTenantNationalHolidayIntegrationEvent @event)
        {
            try
            {
                var context = @event.Context;
                var nationalHolidayCreationMonthFromConfigure = _configuration.GetSection("NationalHolidayCreationMonth").Get<byte>();
                var nationalHolidayCreationMonth = nationalHolidayCreationMonthFromConfigure > 0 ? nationalHolidayCreationMonthFromConfigure : 12;

                var nationalHolidayCreationDayFromConfigure =
                    _configuration.GetSection("NationalHolidayCreationDay").Get<byte>();
                var nationalHolidayCreationDay = nationalHolidayCreationDayFromConfigure > 0 ? nationalHolidayCreationDayFromConfigure : 1;

                var holidays = await _mediator.Send(new CreateTenantNationHolidayCommand(context.TenantId, context.UserId,
                    nationalHolidayCreationMonth, nationalHolidayCreationDay));

                if (holidays != null && holidays.Any())
                {
                    _helper.FlushCacheStore(_cacheClient, @event.Context.RetailerCode, nameof(Holiday));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
