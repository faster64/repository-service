using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class DepartmentWriteOnlyRepository : EfBaseWriteOnlyRepository<Department>, IDepartmentWriteOnlyRepository
    {
        public DepartmentWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<DepartmentWriteOnlyRepository> logger)
            : base(db, authService, logger)
        {
        }
    }
}
