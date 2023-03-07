using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IJobTitleReadOnlyRepository : IBaseReadOnlyRepository<JobTitle, long>
    {
        Task<PagingDataSource<JobTitleDto>> FiltersAsync(ISqlExpression query);
    }
}
