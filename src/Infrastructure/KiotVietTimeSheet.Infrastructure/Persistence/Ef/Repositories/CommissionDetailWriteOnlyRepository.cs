using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class CommissionDetailWriteOnlyRepository : EfBaseWriteOnlyRepository<CommissionDetail>, ICommissionDetailWriteOnlyRepository
    {
        public CommissionDetailWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<CommissionDetailWriteOnlyRepository> logger)
           : base(db, authService, logger)
        {

        }
    }
}
