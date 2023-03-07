using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IDepartmentReadOnlyRepository : IBaseReadOnlyRepository<Department, long>
    {
        Task<PagingDataSource<DepartmentDto>> FiltersAsync(ISqlExpression query);
    }
}
