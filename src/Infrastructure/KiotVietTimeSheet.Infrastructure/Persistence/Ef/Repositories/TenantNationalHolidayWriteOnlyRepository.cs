using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class TenantNationalHolidayWriteOnlyRepository : EfBaseWriteOnlyRepository<TenantNationalHoliday>, ITenantNationalHolidayWriteOnlyRepository
    {
        public TenantNationalHolidayWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<TenantNationalHolidayWriteOnlyRepository> logger)
            : base(db, authService, logger) { }
    }
}
