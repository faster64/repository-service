using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class AllowanceWriteOnlyRepository : EfBaseWriteOnlyRepository<Allowance>, IAllowanceWriteOnlyRepository
    {
        public AllowanceWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<AllowanceWriteOnlyRepository> logger)
            : base(db, authService, logger)
        {
        }
    }
}
