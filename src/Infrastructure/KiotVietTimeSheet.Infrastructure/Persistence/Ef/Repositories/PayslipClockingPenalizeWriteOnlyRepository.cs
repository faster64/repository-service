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
    public class PayslipClockingPenalizeWriteOnlyRepository : EfBaseWriteOnlyRepository<PayslipClockingPenalize>, IPayslipClockingPenalizeWriteOnlyRepository
    {
        public PayslipClockingPenalizeWriteOnlyRepository(
            EfDbContext db, 
            IAuthService authService, 
            ILogger<PayslipClockingPenalizeWriteOnlyRepository> logger) : base(db, authService, logger)
        {
        }

        public async Task CreateOrUpdateAsync(List<Payslip> payslips)
        {
            var payslipIds = payslips.Select(p => p.Id).Distinct().ToList();
            var payslipClockingPenalizesFromSource = await GetBySpecificationAsync(new FindPayslipClockingPenalizeByPayslipIds(payslipIds));
            if (payslipClockingPenalizesFromSource != null && payslipClockingPenalizesFromSource.Any())
            {
                BatchDelete(payslipClockingPenalizesFromSource);
            }

            var listPayslipIdDeleted =
                payslipClockingPenalizesFromSource?.Select(x => x.Id).ToList() ?? new List<long>();

            var payslipClockingPenalizes =
                payslips.Where(x => x.PayslipClockingPenalizes != null)
                    .SelectMany(x => x.PayslipClockingPenalizes)
                    .Where(x => !listPayslipIdDeleted.Contains(x.Id))
                    .Distinct()
                    .ToList();

            if (!payslipClockingPenalizes.Any())
            {
                return;
            }

            BatchAdd(payslipClockingPenalizes);
        }
    }
}
