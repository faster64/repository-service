using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using KiotVietTimeSheet.Infrastructure.Persistence.OrmLite;
using Microsoft.Extensions.Configuration;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{  
    
    public class AutoKeepingWriteOnlyRepository : OrmLiteRepository<Clocking, long>, IAutoKeepingWriteOnlyRepository
    {
        private readonly ILogger<ClockingWriteOnlyRepository> _logger;
        private readonly IConfiguration _configuration;
        public AutoKeepingWriteOnlyRepository(
            IDbConnectionFactory db,
            IAuthService authService,
            IConfiguration configuration,
            ILogger<ClockingWriteOnlyRepository> logger) : base(db, authService)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<List<Clocking>> AutoKeepingAsync(int tenantId, DateTime startTime, DateTime endTime, Guid autoTimekeepingUid,long jobId)
        {
            var clokingUpdated = await Db.SqlListAsync<Clocking>("pr_Auto_Keeping @TenantId, @StartTime, @EndTime,@AutoTimekeepingUid,@JobId",
                        new { 
                            TenantId = tenantId , 
                            StartTime = startTime, 
                            EndTime = endTime,
                            AutoTimekeepingUid = autoTimekeepingUid.ToString("N"),
                            jobId
                        });

            return clokingUpdated;            
        }

    }
}