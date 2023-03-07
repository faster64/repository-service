using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class ShiftWriteOnlyRepository : EfBaseWriteOnlyRepository<Shift>, IShiftWriteOnlyRepository
    {
        public ShiftWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<ShiftWriteOnlyRepository> logger)
           : base(db, authService, logger)
        {

        }
    }
}
