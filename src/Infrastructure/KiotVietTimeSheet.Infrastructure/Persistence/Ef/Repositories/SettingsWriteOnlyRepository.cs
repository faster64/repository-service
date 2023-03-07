using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Linq;
using System;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class SettingsWriteOnlyRepository : EfBaseWriteOnlyRepository<Settings>, ISettingsWriteOnlyRepository
    {
        public SettingsWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<SettingsWriteOnlyRepository> logger)
            : base(db, authService, logger)
        {
        }

        public async Task<bool> UpdateAutoKeepingCronScheduleAsync(Settings setting)
        {
            if (setting == null) return false;

            var isActive = setting.Value.Equals("True") ? true : false;
            var paramTenantId = new SqlParameter("@TenantId", AuthService.Context.TenantId);
            var paramIsActive = new SqlParameter("@IsActive", !isActive);
            var paramIsActiveUpdate = new SqlParameter("@IsActiveUpdate", isActive);

            await Db.Database.ExecuteSqlCommandAsync("UPDATE CronSchedule " +
                                        "SET IsActive = @IsActiveUpdate, " +
                                        "ModifiedDate = GETDATE(), " +
                                        "IsRunning = 0 " +
                                        "WHERE TenantId = @TenantId " +
                                        " AND Type = 1" +
                                        " AND IsActive = @IsActive", paramIsActiveUpdate, paramTenantId, paramIsActive);

            return true;
 
        }

        public async Task<bool> FindAndInsertAutoKeepingCronScheduleAsync(Settings setting)
        {
            if (setting == null) return false;

            var existCronSchedule = await Db.CronSchedule.SingleOrDefaultAsync(x => x.TenantId == AuthService.Context.TenantId && x.Type == 1);
            if (existCronSchedule == null)
            {
                existCronSchedule = new CronSchedule()
                {
                    Type = 1,
                    TenantId = AuthService.Context.TenantId,
                    BranchId = AuthService.Context.BranchId,
                    CreatedBy = (int)(AuthService.Context.User?.Id ?? 0),
                    CreatedDate = DateTime.Now,
                    IsActive = setting.IsActive && setting.Value=="True",
                    IsRunning = false,
                    TenantCode = AuthService.Context.TenantCode,
                    Title = "Tự động chấm công",
                    LastSync = DateTime.Now.AddDays(-1),
                    NextRun = DateTime.Now,
                    LimitRun = DateTime.Now.AddDays(1)
                };
                Db.CronSchedule.Add(existCronSchedule);
                await Db.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
