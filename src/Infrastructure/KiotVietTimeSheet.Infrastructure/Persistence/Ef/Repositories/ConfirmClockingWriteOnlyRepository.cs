using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class ConfirmClockingWriteOnlyRepository : EfBaseWriteOnlyRepository<ConfirmClocking>, IConfirmClockingWriteOnlyRepository
    {
        public ConfirmClockingWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<ConfirmClockingWriteOnlyRepository> logger)
           : base(db, authService, logger)
        {

        }
    }
}
