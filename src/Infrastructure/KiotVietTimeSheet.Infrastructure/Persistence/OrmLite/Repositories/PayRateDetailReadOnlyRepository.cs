using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using ServiceStack.Data;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class PayRateDetailReadOnlyRepository : OrmLiteRepository<PayRateDetail, long>, IPayRateDetailReadOnlyRepository
    {
        public PayRateDetailReadOnlyRepository(IDbConnectionFactory db, IAuthService authService) : base(db, authService) { }

    }
}
