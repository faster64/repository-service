using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class PayRateTemplateDetailWriteOnlyRepository : EfBaseWriteOnlyRepository<PayRateTemplateDetail>, IPayRateTemplateDetailWriteOnlyRepository
    {
        public PayRateTemplateDetailWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<PayRateTemplateDetailWriteOnlyRepository> logger) : base(db, authService, logger)
        {
        }

    }
}
