using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class PayRateTemplateReadOnlyRepository : OrmLiteRepository<PayRateTemplate, long>, IPayRateTemplateReadOnlyRepository
    {
        public PayRateTemplateReadOnlyRepository(IDbConnectionFactory dbFactory, IAuthService authService) : base(dbFactory, authService)
        {

        }

        public virtual async Task<PagingDataSource<PayRateTemplate>> FiltersAsync(ISqlExpression query, bool includeSoftDel = false) => await LoadSelectDataSourceAsync<PayRateTemplate>(query, includeSoftDelete: includeSoftDel);
        public async Task<bool> ExistEmployeeDataAsync(long payRateTemplateId)
        {
            var isExist = await Db.SqlScalarAsync<int>("SELECT COUNT(pr.Id) from PayRate AS pr WITH(NOLOCK)  INNER JOIN Employee AS e ON  pr.EmployeeId = e.Id WHERE ISNULL(e.IsDeleted,0) <> 1 AND PayRateTemplateId =@PayRateTemplateId",new { PayRateTemplateId  = payRateTemplateId });            
            return isExist > 0;
        }
        public async Task<bool> ExistPayrateTemplateBySalaryPeriod(List<byte> salaryPeriod, int retailerId)
        {
            var isExist = await Db.SqlScalarAsync<int>("SELECT COUNT(Id) from PayRateTemplate WITH(NOLOCK) WHERE TenantId =@TenantId AND Status=0 AND SalaryPeriod IN (@SalaryPeriod)", new { TenantId = retailerId, SalaryPeriod = salaryPeriod });
            return isExist > 0;
        }
    }
}
