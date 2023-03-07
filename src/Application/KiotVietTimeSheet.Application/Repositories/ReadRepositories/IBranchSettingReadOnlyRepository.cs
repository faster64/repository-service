using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IBranchSettingReadOnlyRepository : IBaseReadOnlyRepository<BranchSetting, long>
    {
        Task<BranchSetting> FindBranchSettingWithDefault(ISpecification<BranchSetting> spec);
    }
}
