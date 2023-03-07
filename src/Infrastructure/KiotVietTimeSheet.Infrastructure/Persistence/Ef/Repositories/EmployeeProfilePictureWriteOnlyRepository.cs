using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class EmployeeProfilePictureWriteOnlyRepository : EfBaseWriteOnlyRepository<EmployeeProfilePicture>, IEmployeeProfilePictureWriteOnlyRepository
    {
        public EmployeeProfilePictureWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<EmployeeProfilePictureWriteOnlyRepository> logger)
            : base(db, authService, logger)
        {
        }
    }
}
