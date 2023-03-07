using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using Microsoft.EntityFrameworkCore;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class FingerPrintWriteOnlyRepository : EfBaseWriteOnlyRepository<FingerPrint>, IFingerPrintWriteOnlyRepository
    {
        public FingerPrintWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<FingerPrintWriteOnlyRepository> logger) : base(db, authService, logger)
        {
        }

        public async Task<List<FingerPrint>> FindByIdWithoutPermission(long employeeId)
        {
            return await Db.FingerPrint.Where(p => p.TenantId == AuthService.Context.TenantId && p.EmployeeId == employeeId)
                .ToListAsync();
        }
    }
}
