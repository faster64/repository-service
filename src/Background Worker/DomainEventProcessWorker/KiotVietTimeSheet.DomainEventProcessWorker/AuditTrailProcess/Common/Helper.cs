using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Caching;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Validators;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.DomainEventProcessWorker.Persistence;
using KiotVietTimeSheet.Infrastructure.DbMaster.Models;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Microsoft.EntityFrameworkCore;
using ServiceStack;
using ServiceStack.Configuration;

namespace KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Common
{

    public class Helper
    {
        private ILogger<Helper> _logger;
        private ILogger<Helper> Logger => _logger ?? (_logger = HostContext.Resolve<ILogger<Helper>>());

        public async Task<EfDbContext> GetDbContextByGroupId(int groupId, string retailerCode, ICacheClient cacheClient, IKiotVietApiClient kiotVietApiClient, IAppSettings appSettings)
        {
            var keyObject = $@"{CacheKeys.GetEntityCacheKey(
                retailerCode,
                nameof(KiotVietTimeSheet),
                nameof(KvGroup)
            )}*";
            var item = cacheClient.GetOrDefault<KvGroup>(keyObject);
            var timeSheetConnection = appSettings.GetString("ConnectionStrings:KiotVietTimeSheetDatabase");
            var masterDbConnection = appSettings.GetString("ConnectionStrings:KiotVietMasterDatabase");
            if (item == null || string.IsNullOrEmpty(item.TimeSheetConnectionString))
            {
                var optionsMasterDbBuilder = new DbContextOptionsBuilder<KvMasterDataDbContext>();
                optionsMasterDbBuilder.UseSqlServer(masterDbConnection);
                using (var masterDbContext = new KvMasterDataDbContext(optionsMasterDbBuilder.Options))
                {
                    var retailer = await masterDbContext?.KvRetailer.FirstOrDefaultAsync(x => x.Code == retailerCode);
                    if (retailer != null)
                    {
                        var kvGroup = await masterDbContext.KvGroup.FirstOrDefaultAsync(x => x.Id == retailer.GroupId);
                        if (kvGroup != null && !string.IsNullOrEmpty(kvGroup.TimeSheetConnectionString))
                        {
                            cacheClient.Set(keyObject, kvGroup);
                            var shardingConnection = appSettings.GetString("ConnectionStrings:" + kvGroup.TimeSheetConnectionString);
                            if (!string.IsNullOrEmpty(shardingConnection)) timeSheetConnection = shardingConnection;
                        }
                    }
                }
            }
            else
            {
                var shardingConnection = appSettings.GetString("ConnectionStrings:" + item.TimeSheetConnectionString);
                if (!string.IsNullOrEmpty(shardingConnection)) timeSheetConnection = shardingConnection;
            }

            var optionsBuilder = new DbContextOptionsBuilder<EfDbContext>();
            optionsBuilder.UseSqlServer(timeSheetConnection);
            return new EfDbContext(optionsBuilder.Options);
        }

        public async Task<List<Holiday>> CreateNationalHolidayAsync(IntegrationEventContext context, EfDbContext db, IAppSettings appSettings)
        {
            try
            {
                var nationalHolidayCreationMonth = appSettings.Get<byte>("NationalHolidayCreationMonth") > 0 ? appSettings.Get<byte>("NationalHolidayCreationMonth") : 12;
                var nationalHolidayCreationDay = appSettings.Get<byte>("NationalHolidayCreationDay") > 0 ? appSettings.Get<byte>("NationalHolidayCreationDay") : 1;
                var holidays = new List<Holiday>();
                var now = DateTime.Now;
                
                var yearIsCreated = now.Year;

                if (now.Month > nationalHolidayCreationMonth)
                {
                    yearIsCreated = now.AddYears(1).Year;
                }
                else if (now.Month == nationalHolidayCreationMonth && now.Day >= nationalHolidayCreationDay)
                {
                    yearIsCreated = now.AddYears(1).Year;
                }

                var existNationalHoliday = await db.TenantNationalHolidays.AnyAsync(x =>
                    x.LastCreatedYear == yearIsCreated && x.TenantId == context.TenantId);

                if (existNationalHoliday) return holidays;
                
                var nationalHolidays = await db.NationalHolidays.ToListAsync();
                var tenantNationalHoliday =
                    await db.TenantNationalHolidays.FirstOrDefaultAsync(x => x.TenantId == context.TenantId);
                holidays = GenerateHolidays(context, nationalHolidays, yearIsCreated);

                await RemoveHoliday(context, db, yearIsCreated, holidays);

                if (!holidays.Any()) return holidays;

                db.Holidays.AddRange(holidays);

                if (tenantNationalHoliday == null)
                {
                    var tenantNationHoliday = new TenantNationalHoliday(holidays.First().From.Year)
                        { TenantId = context.TenantId };
                    db.TenantNationalHolidays.Add(tenantNationHoliday);
                }
                else
                {
                    tenantNationalHoliday.UpdateLastCreatedYear(holidays.First().From.Year);
                    db.TenantNationalHolidays.Update(tenantNationalHoliday);
                }

                return holidays;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new List<Holiday>();
                // ignored
            }
        }

        private async Task RemoveHoliday(IntegrationEventContext context, EfDbContext db, int yearIsCreated, List<Holiday> holidays)
        {
            var holidayLength = holidays.Count - 1;
            var existedHolidays = await db.Holidays.Where(holiday => (holiday.From.Year == yearIsCreated || holiday.To.Year == yearIsCreated) && holiday.TenantId == context.TenantId).ToListAsync();
            while (holidayLength >= 0)
            {
                var holiday = holidays[holidayLength];
                holidayLength--;

                var validationResult = await new CreateOrUpdateHolidayValidator().ValidateAsync(holiday);

                if (!validationResult.IsValid)
                {
                    holidays.Remove(holiday);
                    continue;
                }

                var isOverlapHoliday = existedHolidays.Any(x => x.To >= holiday.From.Date && x.From <= holiday.To.Date);
                if (isOverlapHoliday) holidays.Remove(holiday);
            }
        }

        private List<Holiday> GenerateHolidays(IntegrationEventContext context, List<NationalHoliday> nationalHolidays, int yearIsCreated)
        {
            var lunarCalendar = new ChineseLunisolarCalendar();

            var holidays = new List<Holiday>();
            nationalHolidays.ForEach(nationalHoliday =>
            {
                Holiday holiday;
                if (nationalHoliday.IsLunarCalendar)
                {
                    var from = lunarCalendar.ToDateTime(yearIsCreated, nationalHoliday.StartMonth,
                        nationalHoliday.StartDay, 0, 0, 0, 0);
                    var to = lunarCalendar.ToDateTime(yearIsCreated, nationalHoliday.EndMonth,
                        nationalHoliday.EndDay, 0, 0, 0, 0);
                    holiday = new Holiday(nationalHoliday.Name, @from, to);
                }
                else
                {
                    holiday = new Holiday(nationalHoliday.Name,
                        new DateTime(yearIsCreated, nationalHoliday.StartMonth, nationalHoliday.StartDay),
                        new DateTime(yearIsCreated, nationalHoliday.EndMonth, nationalHoliday.EndDay));
                }
                holiday.CreatedDate = DateTime.Now;
                holiday.DeletedBy = context.UserId;
                holiday.TenantId = context.TenantId;
                // không tạo lịch sử thao tác khi tự động tạo nghỉ lễ
                holiday.ClearDomainEvents();
                holidays.Add(holiday);
            });
            return holidays;
        }
        public void FlushCacheStore(ICacheClient cacheClient, string tenantCode, string entityName)
        {
            var cleanCacheKeys = new List<string>
                    {
                        $@"{CacheKeys.GetEntityCacheKey(tenantCode, entityName, "*")}",
                        CacheKeys.GetListEntityCacheKey(tenantCode, entityName, "*")
                    };
            if (cleanCacheKeys.Any())
                cacheClient.RemoveByParttern(cleanCacheKeys.ToArray());
        }
    }
}
