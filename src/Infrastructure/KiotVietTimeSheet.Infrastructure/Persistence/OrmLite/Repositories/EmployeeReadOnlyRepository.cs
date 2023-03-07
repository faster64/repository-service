using System;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using Microsoft.AspNetCore.Http.Features;
using KiotVietTimeSheet.Utilities;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class EmployeeReadOnlyRepository : OrmLiteRepository<Employee, long>, IEmployeeReadOnlyRepository
    {
        private readonly IAuthService _authService;

        public EmployeeReadOnlyRepository(IDbConnectionFactory db, IAuthService authService)
            : base(db, authService)
        {
            _authService = authService;
        }

        public virtual async Task<PagingDataSource<Employee>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false, string[] include = null)
        {
            var employeeDataSource = await LoadSelectDataSourceAsync<Employee>(query, include, includeSoftDelete);
            var employeeSort = employeeDataSource.Data.OrderBy(e => e.Name.ToLower().Split(' ').LastOrDefault()).ToList();

            return new PagingDataSource<Employee>
            {
                Total = employeeDataSource.Total,
                Filters = employeeDataSource.Filters,
                Data = employeeSort
            };
        }

        public virtual async Task<Employee> GetByUserIdAsync(long userId, bool reference = false, bool includeSoftDelete = false)
        {
            var employee = await FindBySpecificationWithIncludeAsync(new FindEmployeeByUserIdSpec(userId), new[] { "EmployeeBranches" }, includeSoftDelete);
            return employee;
        }

        /// <inheritdoc />
        /// <summary>
        /// Lấy ra danh sách nhân viên làm thay ca
        /// List nhân viên làm thay ca bao gồm các nhân viên chưa xóa
        /// Thuộc cùng chi nhánh nhân viên không đi làm
        /// Và không có chi tiết làm việc nào chưa hủy trong ca làm việc, ngày làm việc của chi tiết đang đánh dấu không đi làm đó
        /// </summary>
        /// <param name="branchId">Id hiện tại của chi nhánh</param>
        /// <param name="withoutId">Loại bỏ nhân viên đánh dấu không đi làm trong selection</param>
        /// <param name="start"></param>
        /// <param name="endTime"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public virtual async Task<PagingDataSource<Employee>> GetAvailableEmployeesAsync(int branchId, long withoutId, DateTime start, DateTime endTime, int? skip, int? take, string keyword)
        {
            var from = start.Date;
            var to = start.AddDays(1).Date;

            var employeeSpecification = new NotEqualEmployeeIdSpec(withoutId).And(new FindEmployeeByBranchIdSpec(branchId));
            if (!string.IsNullOrEmpty(keyword))
            {
                employeeSpecification = employeeSpecification.And(new FindEmployeeByNameOrCodeSpec(keyword));
            }

            var employees = await GetBySpecificationAsync(employeeSpecification, true);

            // Tìm kiếm tất cả chi tiết làm việc trong ngày
            var clockings = await Db.SelectAsync(Db.From<Clocking>()
                .Where(c => employees.Select(e => e.Id).ToList().Contains(c.EmployeeId) && (c.StartTime >= from && c.StartTime < to)));

            employees.ForEach(e => { e.Clockings = clockings.Where(c => c.EmployeeId == e.Id).ToList(); });

            // Lọc các nhân viên có chi tiết làm việc, trùng khoảng thời gian với ca làm việc và trạng thái khác hủy
            var employeeIds = employees.Where(e => e.Clockings != null && e.Clockings.Any(c => (
                Domain.Utilities.Utilities.IsOverLapTimeBetweenRangeTimes(c.StartTime, c.EndTime, start, endTime)
                && c.ClockingStatus != (byte)ClockingStatuses.Void && !c.IsDeleted
            ))).Select(e => e.Id).ToList();

            employees = employees.Where(e => employeeIds.All(x => x != e.Id)).ToList();

            var count = employees.Count;

            employees = employees.OrderBy(e => e.Name.ToLower().Split(' ').LastOrDefault()).Skip((int)skip).Take((int)take).ToList();

            return new PagingDataSource<Employee>
            {
                Total = count,
                Data = employees
            };
        }

        public async Task<PagingDataSource<Employee>> GetEmployeesMultipleBranch(List<int> branchIds, List<long> shiftIds, List<long?> departmentIds, bool? isActive, bool? isIncludeDelete, string keyword, List<long> employeeIds)
        {
            var query = Db.From<Employee>().Where(x => x.TenantId == _authService.Context.TenantId);

            var queryEmoloyeeIds = new List<long>();
            var employeeBranchIds = (await Db.SelectAsync<EmployeeBranch>(x => x.TenantId == _authService.Context.TenantId && branchIds.Contains(x.BranchId))).Select(x => x.EmployeeId).ToList();
            queryEmoloyeeIds.AddRange(employeeBranchIds);

            if (shiftIds?.Any() == true)
            {
                var clockingEmployeeIds = (await Db.SelectAsync<Clocking>(x => x.TenantId == _authService.Context.TenantId && shiftIds.Contains(x.EmployeeId))).Select(x => x.EmployeeId).ToList();
                queryEmoloyeeIds.AddRange(clockingEmployeeIds);
            }

            if (departmentIds?.Any() == true)
            {
                query = query.Where<Employee>(x => departmentIds.Contains(x.DepartmentId));
            }

            if (employeeIds?.Any() == true)
            {
                queryEmoloyeeIds = queryEmoloyeeIds.Where(x => employeeIds.Contains(x)).ToList();
            }

            var employees = new List<Employee>();

            query = query.Where<Employee>(x => (isIncludeDelete == true || !x.IsDeleted) &&
                                               (isActive == null || isActive == false || x.IsActive) &&
                                               (string.IsNullOrEmpty(keyword) || x.Name.Contains(keyword) || x.MobilePhone.Contains(keyword) || x.Code.Contains(keyword)));

            if (queryEmoloyeeIds.Any())
            {
                queryEmoloyeeIds = queryEmoloyeeIds.Distinct().ToList();

                int i = 0;
                int pageSize = 20;
                var batchEmoloyeeIds = queryEmoloyeeIds.Skip(i).Take(pageSize).ToList();

                while (batchEmoloyeeIds.Any())
                {
                    var batchQuery = query.Clone().Where<Employee>(x => batchEmoloyeeIds.Contains(x.Id));
                    var patchEmployees = await Db.LoadSelectAsync(batchQuery);
                    employees.AddRange(patchEmployees);
                    i++;
                    batchEmoloyeeIds = queryEmoloyeeIds.Skip(i * pageSize).Take(pageSize).ToList();
                }
            }
            else
            {
                employees = await Db.LoadSelectAsync(query);
            }


            return new PagingDataSource<Employee>
            {
                Total = employees.Count,
                Data = employees.OrderBy(x => x.Name.Trim().Split(' ').LastOrDefault()).ToList()
            };
        }

        public async Task<List<Employee>> GetByListEmployeeIdAsync(List<long> employeeIds, bool reference = false, bool includeSoftDelete = false)
        {
            var spec = new GetByLongIdsSpec(employeeIds);
            var limitByTenantId = new LimitByTenantIdSpec<Employee>(AuthService.Context.TenantId);            
            spec.And(limitByTenantId);
            var employees = await Db.LoadSelectAsync(spec.GetExpression(), new[] { "EmployeeBranches" });
            var authorizedBranchIds = await AuthService.GetAuthorizedBranchIds();
            employees = employees.Where(x => authorizedBranchIds.Contains(x.BranchId)||x.EmployeeBranches.Any(y=>authorizedBranchIds.Contains(y.BranchId))).ToList();
            return employees;
        }

        public async Task<Employee> GetEmployeeForClockingGps(int tenantId, string keyword, bool isPhone)
        {
            var employeeQuery = Db.From<Employee>();
            employeeQuery.Where(x => x.TenantId == tenantId);
            employeeQuery.Where(x => !x.IsDeleted);

            if (isPhone)
                employeeQuery.Where(x => x.MobilePhone == keyword);
            else
                employeeQuery.Where(x => x.Code == keyword);

            var employees = await Db.SelectAsync(employeeQuery);
            return employees.FirstOrDefault();
        }

        public async Task<Employee> GetByIdWithoutLimit(long id)
        {
            var query = Db.From<Employee>().Where(x => x.TenantId == _authService.Context.TenantId && !x.IsDeleted && x.Id == id);

            var employees = await Db.SelectAsync(query);
            return employees.FirstOrDefault();
        }

        public async Task<Employee> GetByIdentityKeyClockingWithoutLimit(string identityKeyClocking)
        {
            var query = Db.From<Employee>().Where(x => x.TenantId == _authService.Context.TenantId && !x.IsDeleted && x.IdentityKeyClocking == identityKeyClocking);

            var employees = await Db.SelectAsync(query);
            return employees.FirstOrDefault();
        }

        public async Task<PagingDataSource<Employee>> GetEmployeeWithoutPermission(int retailerId,
            int currentPage, int pageSize, DateTime? lastModifiedFrom)
        {
            var q = Db.From<Employee>();
            q.Where(p => p.TenantId == retailerId);
            if (lastModifiedFrom != null)
            {
                q.Where(x =>
                    (x.ModifiedDate != null && x.ModifiedDate > lastModifiedFrom.Value) ||
                    x.ModifiedDate == null && x.CreatedDate > lastModifiedFrom.Value);
            }
    
            var totalQuery = q.Clone().Select("COUNT (Id)");
            var total = await Db.SingleAsync<long>(totalQuery);
            var listEmployee = new List<Employee>();
            if (total>0)
            {
                q.OrderByDescending(c => c.Id);
                var skip = (currentPage - 1) > 0 ? (currentPage - 1) * pageSize : 0;
                var take = pageSize > 0 ? pageSize : Constant.SyncEmployeeDefaultPageSize;
                q.Limit(skip, take);
                listEmployee = await Db.LoadSelectAsync(q, new[] { "EmployeeBranches" });
            }                
            return new PagingDataSource<Employee>()
            {
                Total = total,
                Data = listEmployee
            };
        }

        public async Task<bool> CheckMobilePhoneExist(long id, string mobileNumber)
        {
            var standardizedMobileNumber = PhoneNumberHelper.StandardizePhoneNumber(mobileNumber, true);
            var query = Db.From<Employee>()
                .Where(x => x.TenantId == _authService.Context.TenantId
                && !x.IsDeleted
                && x.Id != id
                && x.StandardizedMobilePhone == standardizedMobileNumber);

            return await Db.ExistsAsync(query);
        }

    }
}
