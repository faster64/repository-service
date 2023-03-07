using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.SharedKernel.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using System;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class PayslipReadOnlyRepository : OrmLiteRepository<Payslip, long>, IPayslipReadOnlyRepository
    {

        private readonly ILogger<PayslipReadOnlyRepository> _Logger;
        public PayslipReadOnlyRepository(IDbConnectionFactory db, IAuthService authService,ILogger<PayslipReadOnlyRepository> logger) : base(db, authService) 
        {
            _Logger = logger;
        }
        public async Task<PagingDataSource<PayslipDto>> FiltersAsync(ISqlExpression query) => await LoadSelectDataSourceAsync<PayslipDto>(query);
        public async Task<PagingDataSource<Payslip>> GetByQueryFilter(PayslipByPaysheetIdFilter filter)
        {
            ISpecification<Payslip> spec = new DefaultTrueSpec<Payslip>();

            if (filter.PaysheetId.HasValue)
                spec = spec.And(new GetPayslipByPaysheetId(filter.PaysheetId.Value));

            if (filter.EmployeeId.HasValue)
                spec = spec.And(new FindPayslipByEmployeeId(filter.EmployeeId.Value));
            if (filter.PayslipStatuses == null || filter.PayslipStatuses.Count <= 0)
            {
                spec = spec.Not(new FindPayslipByStatusSpec((byte)PayslipStatuses.Draft)).And(new FindPayslipAreNotDraftSpec());
            }
            else
            {
                spec = spec.And(new FindPayslipByStatusesSpec(filter.PayslipStatuses));
            }

            if (filter.PayslipIds != null && filter.PayslipIds.Any())
            {
                spec = spec.And(new FindPayslipByIdsSpec(filter.PayslipIds));
            }

            var query = BuildPagingFilterSqlExpression(filter)
                .And<Payslip>(spec.GetExpression());

            var dataSource = await LoadSelectDataSourceAsync<Payslip>(query);
            return dataSource;
        }

        public async Task<(
            PagingDataSource<Payslip> dataSource,
            decimal totalMainSalary,
            decimal totalCommissionSalary,
            decimal totalOvertimeSalary,
            decimal totalAllowance,
            decimal totalBonus,
            decimal totalDeduction,
            decimal totalNetSalary,
            decimal totalPayment,
            Dictionary<long, long> dicPayslipEmployeeId,
            List<long> employeeIdList)>
            GetByQueryFilterWithSummary(PayslipByPaysheetIdFilter filter)
        {

            ISpecification<Payslip> spec = new DefaultTrueSpec<Payslip>();

            if (filter.PaysheetId.HasValue)
                spec = spec.And(new GetPayslipByPaysheetId(filter.PaysheetId.Value));

            if (filter.EmployeeId.HasValue)
                spec = spec.And(new FindPayslipByEmployeeId(filter.EmployeeId.Value));
            if (filter.PayslipStatuses == null || filter.PayslipStatuses.Count <= 0)
                spec = spec.Not(new FindPayslipByStatusSpec((byte)PayslipStatuses.Draft)).And(new FindPayslipAreNotDraftSpec());
            else
                spec = spec.And(new FindPayslipByStatusesSpec(filter.PayslipStatuses));

            var user = AuthService.Context.User;
            var tenantId = AuthService.Context.TenantId;
            var employeeByUserQuery =
                Db.From<Employee>()
                    .Where(x => x.UserId == user.Id)
                    .Where(x => !x.IsDeleted)
                    .Where(x => x.IsActive)
                    .Where(x => x.TenantId == tenantId);

            var employeeForUser = await Db.SingleAsync(employeeByUserQuery);

            var payslips = await GetBySpecificationAsync(spec);

            var employeeIdList = payslips.Select(p => p.EmployeeId).ToList();

            var query = BuildPagingFilterSqlExpression(filter)
                .And<Payslip>(spec.GetExpression());
            var listQuery = query.Clone().GroupBy(s => s);
            //tach phan quyen voi gioi han giao dich
            var isLimitedByTrans = !user.IsAdmin;
            try
            {
                isLimitedByTrans = await AuthService.HasPermissions(new[] { TimeSheetPermission.EmployeeLimit_Read });
            }
            catch (Exception epx)
            {
                _Logger.LogError(epx.Message, epx);
            }

            //lọc theo người dùng đăng nhập nếu không cho phép xem giao dịch của nhân viên khác
            if (!user.IsAdmin && isLimitedByTrans && employeeForUser == null)
            {
                return (new PagingDataSource<Payslip>(), 0, 0, 0, 0, 0, 0, 0, 0, new Dictionary<long, long>(), new List<long>());
            }

            var querySummary = Db.From<Payslip>(Db.TableAlias(nameof(Payslip)))
                .And<Payslip>(spec.GetExpression())
                .And<Payslip>(x => !x.IsDeleted && x.PayslipStatus != (byte)PayslipStatuses.Void && x.PayslipStatus != (byte)PayslipStatuses.Draft && !x.IsDraft);

            if (!user.IsAdmin && isLimitedByTrans && employeeForUser != null)
            {
                listQuery = listQuery.Where(p => p.EmployeeId == employeeForUser.Id);
                querySummary = querySummary.Where(p => p.EmployeeId == employeeForUser.Id);
            }

            var totalMainSalaryQuery = querySummary
                .Select<Payslip>(payslip => Sql.Sum(payslip.MainSalary));
            var totalMainSalary = await Db.SingleAsync<decimal>(totalMainSalaryQuery);

            var totalCommissionQuery = querySummary
                .Select<Payslip>(payslip => Sql.Sum(payslip.CommissionSalary));
            var totalCommissionSalary = await Db.SingleAsync<decimal>(totalCommissionQuery);

            var totalOvertimeQuery = querySummary
                .Select<Payslip>(payslip => Sql.Sum(payslip.OvertimeSalary));
            var totalOvertimeSalary = await Db.SingleAsync<decimal>(totalOvertimeQuery);

            var totalAllowanceQuery = querySummary
                .Select<Payslip>(payslip => Sql.Sum(payslip.Allowance));
            var totalAllowance = await Db.SingleAsync<decimal>(totalAllowanceQuery);

            var totalBonusQuery = querySummary
                .Select<Payslip>(payslip => Sql.Sum(payslip.Bonus));
            var totalBonus = await Db.SingleAsync<decimal>(totalBonusQuery);

            var totalDeductionQuery = querySummary
                .Select<Payslip>(payslip => Sql.Sum(payslip.Deduction));
            var totalDeduction = await Db.SingleAsync<decimal>(totalDeductionQuery);

            var totalNetSalaryQuery = querySummary
                .Select<Payslip>(payslip => Sql.Sum(payslip.NetSalary));
            var totalNetSalary = await Db.SingleAsync<decimal>(totalNetSalaryQuery);

            var totalPaymentQuery = querySummary
                .Select<Payslip>(payslip => Sql.Sum(payslip.TotalPayment));
            var totalPayment = await Db.SingleAsync<decimal>(totalPaymentQuery);

            var dicPayslipEmployeeId = await Db.DictionaryAsync<long, long>(querySummary.SelectDistinct<Payslip>(payslip => new
            {
                payslip.Id,
                payslip.EmployeeId
            }));

            var dataSource = await LoadSelectDataSourceAsync<Payslip>(listQuery);

            return (dataSource, totalMainSalary, totalCommissionSalary, totalOvertimeSalary, totalAllowance, totalBonus,
                totalDeduction, totalNetSalary, totalPayment, dicPayslipEmployeeId, employeeIdList);
        }

        public async Task<List<Payslip>> GetPayslipByPaysheetId(long paysheetId)
        {
            var query = Db.From<Payslip>()
                .Where(p => p.PaysheetId == paysheetId)
                .Where(p => !p.IsDeleted);
            var result = (await Db.SelectAsync(query));
            return result;
        }
    }
}
