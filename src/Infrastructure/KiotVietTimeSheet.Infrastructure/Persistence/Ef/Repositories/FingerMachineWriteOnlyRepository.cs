using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class FingerMachineWriteOnlyRepository : EfBaseWriteOnlyRepository<FingerMachine>, IFingerMachineWriteOnlyRepository
    {
        public FingerMachineWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<FingerMachineWriteOnlyRepository> logger) : base(db, authService, logger)
        {
        }
    }
}
