using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class EmployeeBranchReadOnlyRepository : OrmLiteRepository<EmployeeBranch, long>, IEmployeeBranchReadOnlyRepository
    {
        public EmployeeBranchReadOnlyRepository(IDbConnectionFactory db, IAuthService authService) : base(db, authService) {

        }
        public async Task<List<EmployeeBranch>> GetBranchIdsByEmployeeIdAsync(long employeeId)
        {
            var qr = Db.From<EmployeeBranch>().Where(x=>x.TenantId == AuthService.Context.TenantId && x.EmployeeId == employeeId);
            var employeeBranchs = await Db.LoadSelectAsync(qr);
            return employeeBranchs;
        }
    }
}
