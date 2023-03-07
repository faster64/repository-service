using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class CommissionWriteOnlyRepository : EfBaseWriteOnlyRepository<Commission>, ICommissionWriteOnlyRepository
    {
        public CommissionWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<CommissionWriteOnlyRepository> logger)
           : base(db, authService, logger)
        {

        }
    }
}
