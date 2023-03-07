using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IEmployeeBranchReadOnlyRepository : IBaseReadOnlyRepository<EmployeeBranch, long>
    {
        Task<List<EmployeeBranch>> GetBranchIdsByEmployeeIdAsync(long employeeId);
    }
}
