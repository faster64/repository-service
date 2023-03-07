using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using ServiceStack.Data;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class PayRateTemplateDetailReadOnlyRepository : OrmLiteRepository<PayRateTemplateDetail, long>, IPayRateTemplateDetailReadOnlyRepository
    {
        public PayRateTemplateDetailReadOnlyRepository(IDbConnectionFactory dbFactory, IAuthService authService) : base(dbFactory, authService)
        {
        }
    }
}
