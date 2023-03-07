using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using Microsoft.EntityFrameworkCore;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class PayRateTemplateWriteOnlyRepository : EfBaseWriteOnlyRepository<PayRateTemplate>, IPayRateTemplateWriteOnlyRepository
    {

        public PayRateTemplateWriteOnlyRepository(
            EfDbContext db,
            IAuthService authService,
            ILogger<PayRateTemplateWriteOnlyRepository> logger
            ) : base(db, authService, logger)
        {
        }

        public async Task<PayRateTemplate> UpdatePayRateTemplateAsync(PayRateTemplate template)
        {
            var existsPayRateTemplateDetails = await Db.PayRateTemplateDetail.Where(t => t.PayRateTemplateId == template.Id).ToListAsync();
            foreach (var existsPayRateTemplateDetail in existsPayRateTemplateDetails)
            {
                if (template.PayRateTemplateDetails.All(pd => pd.Id != existsPayRateTemplateDetail.Id))
                    Db.PayRateTemplateDetail.Remove(existsPayRateTemplateDetail);
            }

            foreach (var payRatePayRateTemplateDetail in template.PayRateTemplateDetails)
            {
                var existsPayRateTemplateDetail = existsPayRateTemplateDetails.FirstOrDefault(f => f.Id > 0 && f.Id == payRatePayRateTemplateDetail.Id);
                if (existsPayRateTemplateDetail != null)
                    Db.PayRateTemplateDetail.Update(payRatePayRateTemplateDetail);
                else
                    Db.PayRateTemplateDetail.Add(payRatePayRateTemplateDetail);
            }
            Update(template);
            await Db.SaveChangesAsync();
            return template;
        }
    }
}
