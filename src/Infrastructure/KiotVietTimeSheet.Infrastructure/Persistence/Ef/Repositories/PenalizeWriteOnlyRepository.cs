using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class PenalizeWriteOnlyRepository : EfBaseWriteOnlyRepository<Penalize>, IPenalizeWriteOnlyRepository
    {
        public PenalizeWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<PenalizeWriteOnlyRepository> logger) : base(db, authService, logger)
        {
        }
    }
}
