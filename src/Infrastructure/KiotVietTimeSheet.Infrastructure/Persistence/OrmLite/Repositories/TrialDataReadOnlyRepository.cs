using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class TrialDataReadOnlyRepository : OrmLiteRepository<Employee, long>, ITrialDataReadOnlyRepository
    {
        public TrialDataReadOnlyRepository(IDbConnectionFactory dbFactory, IAuthService authService) : base(dbFactory, authService)
        {

        }

        public async Task<bool> CreateBookingTrialDataAsync(int tenantId, int branchId, long userIdAdmin, long userId1, long userId2, long commissionId)
        {
            var employees = await Db.SqlListAsync<List<Employee>>("pr_Booking_Insert_TrialData @TenantId, @BranchId, @UserIdAdmin, @UserId1, @UserId2, @CommissionId", 
                        new {
                            TenantId = tenantId,
                            BranchId = branchId,
                            UserIdAdmin = userIdAdmin,
                            UserId1 = userId1,
                            UserId2 = userId2,
                            CommissionId = commissionId
                        });
            if (employees.Count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteBookingTrialDataAsync(int tenantId, long userId)
        {
            var employees = await Db.SqlListAsync<List<Employee>>("pr_Booking_Delete_TrialData @TenantId, @UserId",
                        new
                        {
                            TenantId = tenantId,
                            UserId = userId
                        });
            if (employees.Count == 0)
            {
                return true;
            }
            return false;
        }
    }
}
