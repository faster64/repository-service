using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Application.Repositories.WriteRepositories
{
    public interface IEmployeeWriteOnlyRepository : IBaseWriteOnlyRepository<Employee>
    {
        Task<Employee> UpdateNewAccountSecretKey(long employeeId, long? userId);
        Task<Employee> FindByIdWithoutPermission(long id);
        Task<List<Employee>> GetAllByWithoutPermission(int tenantId, ISpecification<Employee> specification = null);
        Task<bool> AnyWithoutPermission(int tenantId, ISpecification<Employee> specification = null);

    }
}
