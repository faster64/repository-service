using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface ITrialDataReadOnlyRepository : IBaseReadOnlyRepository<Employee, long>
    {
        Task<bool> CreateBookingTrialDataAsync(int tenantId, int branchId, long userIdAdmin, long userId1, long userId2, long commissionId);
        Task<bool> DeleteBookingTrialDataAsync(int tenantId, long userId);
    }
}
