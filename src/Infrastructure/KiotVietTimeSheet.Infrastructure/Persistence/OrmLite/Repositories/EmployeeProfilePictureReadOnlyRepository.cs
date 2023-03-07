using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using ServiceStack.Data;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class EmployeeProfilePictureReadOnlyRepository : OrmLiteRepository<EmployeeProfilePicture, long>, IEmployeeProfilePictureReadOnlyRepository
    {
        public EmployeeProfilePictureReadOnlyRepository(IDbConnectionFactory db, IAuthService authService)
            : base(db, authService)
        {

        }
    }
}
