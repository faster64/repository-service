using System.Collections.Generic;
using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class PayslipWriteOnlyRepository : EfBaseWriteOnlyRepository<Payslip>, IPayslipWriteOnlyRepository
    {
        public PayslipWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<PayslipWriteOnlyRepository> logger)
            : base(db, authService, logger) { }


        public void RemovePayslipClockings(List<PayslipClocking> payslipClockings)
        {
            Db.PayslipClockings.RemoveRange(payslipClockings);
        }

        public async Task UpdateTotalPaymentAsync(Payslip payslip)
        {
            var param = new List<SqlParameter> {
                new SqlParameter("@totalPayment", payslip.TotalPayment),
                new SqlParameter("@tenantId", payslip.TenantId),
                new SqlParameter("@payslipId", payslip.Id)
            };
            await Db.Database.ExecuteSqlCommandAsync($"UPDATE Payslip SET TotalPayment=@totalPayment WHERE TenantId=@tenantId AND Id=@payslipId", param);
        }
    }
}
