using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class FingerPrintReadOnlyRepository : OrmLiteRepository<FingerPrint, long>, IFingerPrintReadOnlyRepository
    {
        public FingerPrintReadOnlyRepository(IDbConnectionFactory dbFactory, IAuthService authService) : base(dbFactory, authService)
        {
        }

        public async Task<PagingDataSource<FingerPrint>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false)
        {
            var fingerMachineDataSource = await LoadSelectDataSourceAsync<FingerPrint>(query, null, includeSoftDelete);
            var data = fingerMachineDataSource.Data.ToList();

            return new PagingDataSource<FingerPrint>
            {
                Total = fingerMachineDataSource.Total,
                Filters = fingerMachineDataSource.Filters,
                Data = data
            };
        }
    }
}
