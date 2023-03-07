using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class CommissionBranchWriteOnlyRepository : EfBaseWriteOnlyRepository<CommissionBranch>, ICommissionBranchWriteOnlyRepository
    {
        public CommissionBranchWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<CommissionBranchWriteOnlyRepository> logger)
            : base(db, authService, logger)
        {
        }
    }
}
