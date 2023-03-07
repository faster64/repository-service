using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IFingerPrintReadOnlyRepository : IBaseReadOnlyRepository<FingerPrint, long>
    {
        Task<PagingDataSource<FingerPrint>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false);
    }
}
