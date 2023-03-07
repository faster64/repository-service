using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class JobTitleWriteOnlyRepository : EfBaseWriteOnlyRepository<JobTitle>, IJobTitleWriteOnlyRepository
    {
        public JobTitleWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<JobTitleWriteOnlyRepository> logger)
            : base(db, authService, logger)
        {
        }
    }
}
