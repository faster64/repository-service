using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Extensions;
using KiotVietTimeSheet.SharedKernel.Specifications;
using ServiceStack;
using System.Linq;
using KiotVietTimeSheet.SharedKernel.QueryFilter;
using System;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class PaysheetReadOnlyRepository : OrmLiteRepository<Paysheet, long>, IPaysheetReadOnlyRepository
    {
        private readonly ILogger<PaysheetReadOnlyRepository> _Logger;
        public PaysheetReadOnlyRepository(IDbConnectionFactory db, IAuthService authService, ILogger<PaysheetReadOnlyRepository> logger) : base(db, authService) 
        {
            _Logger = logger;
        }
        public async Task<PagingDataSource<Paysheet>> FiltersAsync(ISqlExpression query) => await LoadSelectDataSourceAsync<Paysheet>(query);

        public async Task<(PagingDataSource<Paysheet> dataSource, decimal totalNetSalary, decimal totalPayment, Dictionary<long, List<long>> dicPaysheetEmployeeIds)>  GetListByQueryFilterAsync(PaySheetQueryFilter filter, bool includePaySlips = false)
        {
            var authorizedBranchIds = await AuthService.GetAuthorizedBranchIds();
            var specification = GenSpeccificationPaysheet(filter, authorizedBranchIds);
                        
            var user = AuthService.Context.User;
            var tenantId = AuthService.Context.TenantId;
            var employeeByUserQuery =
                Db.From<Employee>()
                    .Where(x => x.UserId == user.Id)
                    .Where(x => !x.IsDeleted)
                    .Where(x => x.IsActive)
                    .Where(x => x.TenantId == tenantId);
            var employeeForUser = await Db.SingleAsync(employeeByUserQuery);
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
            if (!user.IsAdmin && isLimitedByTrans && employeeForUser == null)
            {
                return (new PagingDataSource<Paysheet>(), 0, 0, new Dictionary<long, List<long>>());
            }

            var query = Db.From<Paysheet>(Db.TableAlias(typeof(Paysheet).Name))
                .LeftJoin<Payslip>((paySheet, payslip) => paySheet.Id == payslip.PaysheetId && !payslip.IsDraft)
                .LeftJoin<Payslip, Employee>((payslip, employee) => payslip.EmployeeId == employee.Id)
                .Where<Paysheet>(specification.GetExpression());

            var querySummary = Db.From<Paysheet>(Db.TableAlias(typeof(Paysheet).Name))
                .Join<Payslip>((paySheet, payslip) => paySheet.Id == payslip.PaysheetId)
                .Where<Paysheet>(specification.GetExpression())
                .And<Payslip>(x => !x.IsDeleted)
                .And<Payslip>(x => x.PayslipStatus != (byte)PayslipStatuses.Void)
                .And<Payslip>(x => x.PayslipStatus != (byte)PayslipStatuses.Draft);

            if (!user.IsAdmin && isLimitedByTrans && employeeForUser != null)
            {
                query = query.Where<Payslip>(p => p.EmployeeId == employeeForUser.Id);
                querySummary = querySummary.Where<Payslip>(p => p.EmployeeId == employeeForUser.Id);
            }

            if (!string.IsNullOrEmpty(filter.EmployeeKeyword))
            {
                // Lấy ra các paysheet chứa các payslip của các nhân viên phù hợp vs keyword
                var queryPaySheetIds = 
                    Db.From<Payslip>(Db.TableAlias(typeof(Payslip).Name))
                    .And<Payslip>(x => !x.IsDeleted)
                    .And<Payslip>(x => x.PayslipStatus != (byte)PayslipStatuses.Void)
                    .And<Payslip>(x => x.PayslipStatus != (byte)PayslipStatuses.Draft)
                    .And<Payslip>(x => !x.IsDraft)
                    .And<Payslip>(x => x.TenantId == AuthService.Context.TenantId)
                    .Join<Payslip, Employee>(
                        (payslip, employee) => payslip.EmployeeId == employee.Id
                    )
                    .And<Employee>(employee => employee.Name.Contains(filter.EmployeeKeyword))
                    .Or<Employee>(employee => employee.Code.Contains(filter.EmployeeKeyword))
                    .SelectDistinct(x => x.PaysheetId);

                var paySheetIds = (await Db.SelectAsync<long>(queryPaySheetIds)).Select(x => long.Parse(x.SqlValue())).ToList();
                query = query.And(new FindPaysheetByIdsSpec(paySheetIds).GetExpression());
                querySummary = querySummary.And(new FindPaysheetByIdsSpec(paySheetIds).GetExpression());
            }

            var totalQuery = query.Clone()
                .GroupBy(s => s.Id)
                .Select("COUNT(*) OVER ()");

            var listQuery = GenListQuery(query, filter);

            var resultIncludePayslips = await GetPaysheets(listQuery, includePaySlips);

            var data = (await Db.SelectAsync<object>(listQuery)).ConvertTo<List<Paysheet>>();

            if (includePaySlips) data = resultIncludePayslips;

            var dataSource = new PagingDataSource<Paysheet>
            {
                Total = await Db.SingleAsync<long>(totalQuery),
                Data = data
            };

            querySummary.ClearLimits();

            var totalNetSalaryQuery = querySummary
                .Select<Paysheet, Payslip>(
                    (paySheet, payslip) => 
                        Sql.Sum(!payslip.IsDeleted 
                                && payslip.PayslipStatus != (byte)PaysheetStatuses.Void 
                                && payslip.PayslipStatus != (byte)PaysheetStatuses.Draft
                                && payslip.PayslipStatus != (byte)PaysheetStatuses.Pending ? payslip.NetSalary : 0));

            var totalNetSalary = await Db.SingleAsync<decimal>(totalNetSalaryQuery);

            var totalPaymentQuery = querySummary
                .Select<Paysheet, Payslip>((paySheet, payslip) => Sql.Sum(payslip.TotalPayment));
            var totalPayingAmount = await Db.SingleAsync<decimal>(totalPaymentQuery);

            var dicPaysheetEmployeeIds = await Db.LookupAsync<long, long>(querySummary
                .Where<Paysheet, Payslip>((paySheet, payslip) => !paySheet.IsDraft)
                .Where<Paysheet, Payslip>((paySheet, payslip) => paySheet.PaysheetStatus == (byte)PaysheetStatuses.TemporarySalary)
                .SelectDistinct<Paysheet, Payslip>((paySheet, payslip) => new
                {
                    payslip.PaysheetId,
                    payslip.EmployeeId
                }));

            return (dataSource, totalNetSalary, totalPayingAmount, dicPaysheetEmployeeIds);
        }

        private ISpecification<Paysheet> GenSpeccificationPaysheet(PaySheetQueryFilter filter, IList<int> authorizedBranchIds)
        {
            var specification = new DefaultTrueSpec<Paysheet>().Not(new FindPaysheetByStatusSpec((byte)PaysheetStatuses.Draft));
            specification = specification.And(new FindPaysheetByTenantIdSpec(AuthService.Context.TenantId));

            if (!string.IsNullOrEmpty(filter.PaysheetKeyword))
                specification = specification.And(new FindPaysheetByNameOrCode(filter.PaysheetKeyword));

            if (filter.BranchIds != null && filter.BranchIds.Any())
                specification = specification.And(new FindPaysheetByBranchIds(filter.BranchIds));

            if (filter.PaySheetIds != null && filter.PaySheetIds.Any())
            {
                specification = specification.And(new FindPaysheetByIdsSpec(filter.PaySheetIds));
            }
            else
            {
                if (filter.SalaryPeriod.HasValue)
                    specification = specification.And(new FindPaysheetBySalaryPeriod(filter.SalaryPeriod.Value));

                if (filter.StartTime.HasValue && filter.EndTime.HasValue)
                    specification = specification.And(new FindPaysheetByTimeRange(filter.StartTime.Value.Date,
                        filter.EndTime.Value.AddDays(1).Date));

                if (filter.PaysheetStatuses != null && filter.PaysheetStatuses.Any())
                    specification = specification.And(new FindPaysheetByStatuses(filter.PaysheetStatuses));
            }

            specification =
                specification.And(
                    new LimitByBranchAccessSpec<Paysheet>(authorizedBranchIds.ToList()));

            return specification;
        }

        private async Task<List<Paysheet>> GetPaysheets(SqlExpression<Paysheet> listQuery, bool includePaySlips)
        {
            var resultIncludePayslips = new List<Paysheet>();
            if (!includePaySlips) return resultIncludePayslips;

            var result = (await Db.SelectAsync(listQuery)).ConvertTo<List<Paysheet>>();
            var paySheetIds = result.Select(s => s.Id);

            var queryPayslip =
                Db.From<Payslip>(Db.TableAlias(typeof(Payslip).Name)).Where(x =>
                    paySheetIds.Contains(x.PaysheetId) && !x.IsDeleted &&
                    x.PayslipStatus != (byte) PayslipStatuses.Void &&
                    x.PayslipStatus != (byte) PayslipStatuses.Draft);

            var payslips = (await Db.SelectAsync(queryPayslip)).ConvertTo<List<Payslip>>();

            var payslipIds = payslips.Select(p => p.Id).ToList();
            var queryPayslipDetail = Db.From<PayslipDetail>(Db.TableAlias(typeof(PayslipDetail).Name)).Where(x => payslipIds.Contains(x.PayslipId));
            var payslipsDetails = (await Db.SelectAsync(queryPayslipDetail)).ConvertTo<List<PayslipDetail>>();

            foreach (var paySheet in result)
            {
                paySheet.Payslips = payslips.Where(x => x.PaysheetId == paySheet.Id).ToList();

                if (!paySheet.Payslips.Any()) continue;

                foreach (var payslip in paySheet.Payslips)
                {
                    payslip.PayslipDetails = payslipsDetails.Where(p => p.PayslipId == payslip.Id).ToList();
                }
            }

            resultIncludePayslips = result;
            return resultIncludePayslips;
        }

        private static SqlExpression<Paysheet> GenListQuery(SqlExpression<Paysheet> query, IQueryFilter filter)
        {
            var listQuery = query.Clone().GroupBy(s => s).PagingFilter(filter);

            //TODOs: need write other query. 
            //Get all value and then group all this to calculate
            return listQuery.Select<Paysheet, Payslip>((s, ps) => new
            {
                s.Id,
                s.BranchId,
                s.Code,
                s.IsDeleted,
                s.ModifiedBy,
                s.ModifiedDate,
                s.TenantId,
                s.CreatedBy,
                s.CreatedDate,
                s.CreatorBy,
                s.EndTime,
                s.Name,
                s.Note,
                s.StartTime,
                s.PaysheetStatus,
                s.PaysheetPeriodName,
                s.WorkingDayNumber,
                s.SalaryPeriod,
                s.PaysheetCreatedDate,
                s.Version,
                s.TimeOfStandardWorkingDay,
                TotalPayment = Sql.Sum(!ps.IsDeleted && ps.PayslipStatus != (byte)PaysheetStatuses.Void 
                                                     && ps.PayslipStatus != (byte)PaysheetStatuses.Draft ? ps.TotalPayment : 0),

                TotalNetSalary = Sql.Sum(!ps.IsDeleted && ps.PayslipStatus != (byte)PaysheetStatuses.Void 
                                                       && ps.PayslipStatus != (byte)PaysheetStatuses.Draft ? ps.NetSalary : 0),

                TotalRefund = Sql.Sum(!ps.IsDeleted && ps.PayslipStatus != (byte)PaysheetStatuses.Void 
                                                    && ps.PayslipStatus != (byte)PaysheetStatuses.Draft ? (ps.NetSalary - ps.TotalPayment >= 0 ? 0 : ps.NetSalary - ps.TotalPayment) : 0), //NOSONAR

                EmployeeTotal = Sql.Sum(!ps.IsDeleted && ps.PayslipStatus != (byte)PaysheetStatuses.Void 
                                                      && ps.PayslipStatus != (byte)PaysheetStatuses.Draft ? 1 : 0),
                s.ErrorStatus
            });
        }

        public async Task<Paysheet> GetPaysheetById(long paysheetId)
        {
            var query = Db.From<Paysheet>().Where(p => p.Id == paysheetId);
            var result = (await Db.SelectAsync(query)).FirstOrDefault();
            return result;
        }
        public async Task<Paysheet> GetPaysheetWithoutVoidPayslipById(long paysheetId)
        {
            var paysheet = await FindByIdAsync(paysheetId, true);
            paysheet.Payslips = paysheet.Payslips.Where(x => x.PayslipStatus != (byte)PayslipStatuses.Void).ToList();
            return paysheet;
        }
    }
}
