using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class PayslipDetailWriteOnlyRepository : EfBaseWriteOnlyRepository<PayslipDetail>, IPayslipDetailWriteOnlyRepository
    {
        public PayslipDetailWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<PayslipDetailWriteOnlyRepository> logger) : base(db, authService, logger) { }
    }
}
