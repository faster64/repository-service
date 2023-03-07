using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using System.Threading.Tasks;
using KiotVietTimeSheet.Utilities;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Application.Runtime.Exception;
using KiotVietTimeSheet.SharedKernel.Specifications;
using Microsoft.EntityFrameworkCore;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class EmployeeWriteOnlyRepository : EfBaseWriteOnlyRepository<Employee>, IEmployeeWriteOnlyRepository
    {
        public EmployeeWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<EmployeeWriteOnlyRepository> logger)
           : base(db, authService, logger)
        {

        }

        public async Task<Employee> UpdateNewAccountSecretKey(long employeeId, long? userId)
        {
            var emp = await FindByIdAsync(employeeId);
            if (emp == null) throw new KvTimeSheetException("Can not find employee");

            emp.AccountSecretKey = await GetNewAccountSecretKey();
            emp.SecretKeyTakenUserId = userId;

            Update(emp);
            await UnitOfWork.CommitAsync();

            return emp;
        }

        public async Task<Employee> FindByIdWithoutPermission(long id)
        {
            return await Db.Employees.FirstOrDefaultAsync(p => p.TenantId == AuthService.Context.TenantId && p.Id == id && !p.IsDeleted);
        }

        public async Task<List<Employee>> GetAllByWithoutPermission(int tenantId, ISpecification<Employee> specification = null)
        { 
            var query = Db.Employees.Where(p => p.TenantId == tenantId);
            if (specification != null)
            {
                query = query.Where(specification.GetExpression());
            }
            return await query.ToListAsync();
        }

        public async Task<bool> AnyWithoutPermission(int tenantId, ISpecification<Employee> specification = null)
        {
            var query = Db.Employees.Where(p => p.TenantId == tenantId);
            if (specification != null)
            {
                query = query.Where(specification.GetExpression());
            }

            return await query.AnyAsync();
        }


        private async Task<string> GetNewAccountSecretKey()
        {
            var key = string.Empty;
            var loopCount = 0;
            var isExist = true;

            while (isExist && loopCount <= 100)
            {
                key = Globals.RandomString(8);
                isExist = await AnyBySpecificationAsync(new FindEmployeeByAccountSecretKeySpec(key));
                loopCount++;
            }

            if (isExist) throw new KvTimeSheetException("Can not get new key.");

            return EncryptHelpers.RijndaelEncrypt(key);
        }
    }
}
