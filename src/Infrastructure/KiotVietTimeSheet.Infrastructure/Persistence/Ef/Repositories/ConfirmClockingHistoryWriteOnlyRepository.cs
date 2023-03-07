using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class ConfirmClockingHistoryWriteOnlyRepository : EfBaseWriteOnlyRepository<ConfirmClockingHistory>, IConfirmClockingHistoryWriteOnlyRepository
    {
        public ConfirmClockingHistoryWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<ConfirmClockingHistoryWriteOnlyRepository> logger)
           : base(db, authService, logger)
        {
        }
    }
}
