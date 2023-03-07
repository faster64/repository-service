using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using ServiceStack.Data;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class PayslipDetailReadOnlyRepository : OrmLiteRepository<PayslipDetail, long>, IPayslipDetailReadOnlyRepository
    {
        public PayslipDetailReadOnlyRepository(IDbConnectionFactory db, IAuthService authService) : base(db, authService) { }

    }
}
