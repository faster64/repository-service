using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using Microsoft.EntityFrameworkCore;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class PayRateWriteOnlyRepository : EfBaseWriteOnlyRepository<PayRate>, IPayRateWriteOnlyRepository
    {

        public PayRateWriteOnlyRepository(
            EfDbContext db,
            IAuthService authService,
            ILogger<PayRateWriteOnlyRepository> logger
            ) : base(db, authService, logger)
        { }

        public async Task<PayRate> UpdatePayRateAsync(PayRate payRate)
        {
            var existsPayRateDetails = await Db.PayRateDetail.Where(t => t.PayRateId == payRate.Id).ToListAsync();
            foreach (var existsPayRateDetail in existsPayRateDetails)
            {
                if (payRate.PayRateDetails.All(pd => pd.Id != existsPayRateDetail.Id))
                    Db.PayRateDetail.Remove(existsPayRateDetail);
            }

            foreach (var payRatePayRateDetail in payRate.PayRateDetails)
            {
                var existsPayRateDetail = existsPayRateDetails.FirstOrDefault(f => f.Id > 0 && f.Id == payRatePayRateDetail.Id);
                if (existsPayRateDetail != null)
                    Db.PayRateDetail.Update(payRatePayRateDetail);
                else
                    Db.PayRateDetail.Add(payRatePayRateDetail);
            }
            Update(payRate);
            await Db.SaveChangesAsync();
            return payRate;
        }

        public async Task<List<PayRate>> BatchUpdatePayRateAsync(List<PayRate> payRates)
        {
            var payRateIds = payRates.Select(p => p.Id).ToList();
            var existPayRates = await Db.PayRate.Where(x => payRateIds.Contains(x.Id)).Include(p => p.PayRateDetails).ToListAsync();
            payRates = 
                (from pr in payRates
                from epr in existPayRates
                where pr.Id == epr.Id
                select pr).ToList();

            var payRateId = payRates.Select(x => x.Id).ToList();
            var existPayRateDetails = await Db.PayRateDetail.Where(t => payRateId.Contains(t.PayRateId)).ToListAsync();

            foreach (var payRate in payRates)
            {
                var existPayRateDetailFromSources = existPayRateDetails.Where(t => t.PayRateId == payRate.Id).ToList();
                foreach (var existsPayRateDetail in existPayRateDetailFromSources.Where(existsPayRateDetail => payRate.PayRateDetails.All(pd => pd.Id != existsPayRateDetail.Id)))
                {
                    Db.PayRateDetail.Remove(existsPayRateDetail);
                }

                foreach (var payRatePayRateDetail in payRate.PayRateDetails)
                {
                    var existsPayRateDetail = existPayRateDetailFromSources.FirstOrDefault(f => f.Id > 0 && f.Id == payRatePayRateDetail.Id);
                    if (existsPayRateDetail != null)
                        Db.PayRateDetail.Update(payRatePayRateDetail);
                    else
                        Db.PayRateDetail.Add(payRatePayRateDetail);
                }
                var detachedPayRate = DetachByClone(payRate);

                Update(detachedPayRate);
                
            }

            await Db.SaveChangesAsync();
            return payRates;
        }
    }
}
