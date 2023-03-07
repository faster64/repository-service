using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class EmployeeBranchWriteOnlyRepository : EfBaseWriteOnlyRepository<EmployeeBranch>,
        IEmployeeBranchWriteOnlyRepository
    {
        public EmployeeBranchWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<EmployeeBranchWriteOnlyRepository> logger)
            : base(db, authService, logger)
        {
        }
    }
}
