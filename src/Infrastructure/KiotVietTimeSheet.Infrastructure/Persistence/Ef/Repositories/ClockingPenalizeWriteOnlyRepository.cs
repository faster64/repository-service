using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class ClockingPenalizeWriteOnlyRepository : EfBaseWriteOnlyRepository<ClockingPenalize>, IClockingPenalizeWriteOnlyRepository
    {
        public ClockingPenalizeWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<ClockingPenalizeWriteOnlyRepository> logger) : base(db, authService, logger)
        {
        }
    }
}
