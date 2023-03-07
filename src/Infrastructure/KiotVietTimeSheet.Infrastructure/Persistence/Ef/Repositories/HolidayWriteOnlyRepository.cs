using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class HolidayWriteOnlyRepository : EfBaseWriteOnlyRepository<Holiday>, IHolidayWriteOnlyRepository
    {
        public HolidayWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<HolidayWriteOnlyRepository> logger)
            : base(db, authService, logger)
        {
        }
    }
}
