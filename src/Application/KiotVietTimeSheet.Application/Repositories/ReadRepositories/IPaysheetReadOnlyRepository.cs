using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IPaysheetReadOnlyRepository : IBaseReadOnlyRepository<Paysheet, long>
    {
        Task<PagingDataSource<Paysheet>> FiltersAsync(ISqlExpression query);
        Task<(PagingDataSource<Paysheet> dataSource, decimal totalNetSalary, decimal totalPayment, Dictionary<long, List<long>> dicPaysheetEmployeeIds)> GetListByQueryFilterAsync(PaySheetQueryFilter filter, bool includePaySlips = false);
        Task<Paysheet> GetPaysheetById(long paysheetId);
        Task<Paysheet> GetPaysheetWithoutVoidPayslipById(long paysheetId);
    }
}
