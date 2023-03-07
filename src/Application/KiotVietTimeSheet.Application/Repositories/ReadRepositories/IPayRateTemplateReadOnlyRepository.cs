using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IPayRateTemplateReadOnlyRepository : IBaseReadOnlyRepository<PayRateTemplate, long>
    {
        Task<PagingDataSource<PayRateTemplate>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false);
        Task<bool> ExistEmployeeDataAsync(long payRateTemplateId);
        Task<bool> ExistPayrateTemplateBySalaryPeriod(List<byte> salaryPeriod, int retailerId);
    }
}
