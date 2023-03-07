using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IPayRateReadOnlyRepository : IBaseReadOnlyRepository<PayRate, long>
    {
        Task<PagingDataSource<PayRateDto>> FiltersAsync(ISqlExpression query);
        Task<List<PayRate>> FiltersAsync(List<long> payrateTemplateIdList);
        Task<(bool, string)> CheckPayRateTemplateDeleteByEmployeeIds(List<long> employeeIds);
    }
}
