using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class PayslipPenalizeWriteOnlyRepository : EfBaseWriteOnlyRepository<PayslipPenalize>, IPayslipPenalizeWriteOnlyRepository
    {
        public PayslipPenalizeWriteOnlyRepository(
            EfDbContext db, 
            IAuthService authService, 
            ILogger<PayslipPenalizeWriteOnlyRepository> logger) : base(db, authService, logger)
        {
        }

        public async Task DeleteAllAsync(List<long> payslipIds)
        {
            var payslipPenalizesFromSource = await GetBySpecificationAsync(new FindPayslipPenalizeByPayslipIds(payslipIds));
            if (payslipPenalizesFromSource != null && payslipPenalizesFromSource.Any())
            {
                BatchDelete(payslipPenalizesFromSource);
            }
        }
    }
}
