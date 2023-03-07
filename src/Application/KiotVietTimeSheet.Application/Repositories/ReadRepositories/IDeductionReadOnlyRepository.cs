using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IDeductionReadOnlyRepository : IBaseReadOnlyRepository<Deduction, long>
    {
        Task<PagingDataSource<DeductionDto>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false);
    }
}
