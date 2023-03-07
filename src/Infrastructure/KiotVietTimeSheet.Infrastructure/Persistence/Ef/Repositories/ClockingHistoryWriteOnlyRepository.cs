using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class ClockingHistoryWriteOnlyRepository : EfBaseWriteOnlyRepository<ClockingHistory>, IClockingHistoryWriteOnlyRepository
    {
        public ClockingHistoryWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<ClockingHistoryWriteOnlyRepository> logger)
            : base(db, authService, logger)
        {
        }
    }
}
