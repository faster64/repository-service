using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class DeductionWriteOnlyRepository : EfBaseWriteOnlyRepository<Deduction>, IDeductionWriteOnlyRepository
    {
        public DeductionWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<DeductionWriteOnlyRepository> logger) : base(db, authService, logger)
        {
        }
    }
}
