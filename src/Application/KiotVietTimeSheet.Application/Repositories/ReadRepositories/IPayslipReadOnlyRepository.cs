using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IPayslipReadOnlyRepository : IBaseReadOnlyRepository<Payslip, long>
    {
        Task<PagingDataSource<PayslipDto>> FiltersAsync(ISqlExpression query);
        Task<PagingDataSource<Payslip>> GetByQueryFilter(PayslipByPaysheetIdFilter filter);

        Task<(PagingDataSource<Payslip> dataSource, decimal totalMainSalary, decimal totalCommissionSalary, decimal
            totalOvertimeSalary, decimal totalAllowance, decimal totalBonus, decimal totalDeduction, decimal
            totalNetSalary, decimal totalPayment, Dictionary<long, long> dicPayslipEmployeeId, List<long> employeeIdList)> GetByQueryFilterWithSummary(
            PayslipByPaysheetIdFilter filter);

        Task<List<Payslip>> GetPayslipByPaysheetId(long paysheetId);
    }
}
