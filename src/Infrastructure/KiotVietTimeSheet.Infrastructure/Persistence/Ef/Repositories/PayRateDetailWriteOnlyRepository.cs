using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class PayRateDetailWriteOnlyRepository : EfBaseWriteOnlyRepository<PayRateDetail>, IPayRateDetailWriteOnlyRepository
    {
        public PayRateDetailWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<PayRateDetailWriteOnlyRepository> logger)
            : base(db, authService, logger)
        {
        }

    }
}
