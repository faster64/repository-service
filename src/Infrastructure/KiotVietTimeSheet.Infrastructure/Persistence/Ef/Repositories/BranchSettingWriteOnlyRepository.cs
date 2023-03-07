using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class BranchSettingWriteOnlyRepository : EfBaseWriteOnlyRepository<BranchSetting>, IBranchSettingWriteOnlyRepository
    {
        public BranchSettingWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<BranchSettingWriteOnlyRepository> logger)
            : base(db, authService, logger)
        {
        }
    }
}
