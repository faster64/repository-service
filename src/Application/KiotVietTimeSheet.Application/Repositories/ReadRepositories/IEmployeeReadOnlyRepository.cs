using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using System;
using System.Threading.Tasks;
using ServiceStack.OrmLite;
using KiotVietTimeSheet.SharedKernel.Models;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IEmployeeReadOnlyRepository : IBaseReadOnlyRepository<Employee, long>
    {
        Task<PagingDataSource<Employee>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false, string[] include = null);

        /// <summary>
        /// Lấy ra danh sách nhân viên làm thay ca
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="withoutId"></param>
        /// <param name="start"></param>
        /// <param name="endTime"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        Task<PagingDataSource<Employee>> GetAvailableEmployeesAsync(int branchId, long withoutId, DateTime start, DateTime endTime, int? skip, int? take, string keyword);
        Task<PagingDataSource<Employee>> GetEmployeesMultipleBranch(List<int> branchIds, List<long> shiftIds, List<long?> departmentIds, bool? isActive, bool? isIncludeDelete, string keyword, List<long> employeeIds);

        Task<Employee> GetByUserIdAsync(long userId, bool reference = false, bool includeSoftDelete = false);
        Task<List<Employee>> GetByListEmployeeIdAsync(List<long> employeeIds, bool reference = false, bool includeSoftDelete = false);
        Task<Employee> GetEmployeeForClockingGps(int tenantId, string keyword, bool isPhone);
        Task<Employee> GetByIdWithoutLimit(long id);
        Task<Employee> GetByIdentityKeyClockingWithoutLimit(string identityKeyClocking);

        Task<PagingDataSource<Employee>> GetEmployeeWithoutPermission(int retailerId, int currentPage,
            int pageSize, DateTime? lastModifiedFrom);
        Task<bool> CheckMobilePhoneExist(long id, string mobileNumber);
    }
}
