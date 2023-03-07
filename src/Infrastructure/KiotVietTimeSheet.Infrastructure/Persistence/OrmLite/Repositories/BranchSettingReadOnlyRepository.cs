using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using ServiceStack.Data;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class BranchSettingReadOnlyRepository : OrmLiteRepository<BranchSetting, long>, IBranchSettingReadOnlyRepository
    {
        public BranchSettingReadOnlyRepository(IDbConnectionFactory db, IAuthService authService)
           : base(db, authService)
        {

        }
        public async Task<BranchSetting> FindBranchSettingWithDefault(ISpecification<BranchSetting> spec)
        {

            var branchSetting = await FindBySpecificationAsync(spec);
            if (branchSetting != null) return branchSetting;
            var result = new BranchSetting();
            result.SetDefaultBranchSetting();
            return result;
        }
    }
}
