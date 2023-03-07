using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Specifications;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class TenantNationalHolidayReadOnlyRepository : OrmLiteRepository<TenantNationalHoliday, int>, ITenantNationalHolidayReadOnlyRepository
    {
        public TenantNationalHolidayReadOnlyRepository(
            IDbConnectionFactory db,
            IAuthService authService)
            : base(db, authService) { }

        public async Task<TenantNationalHoliday> GetByTenantIdAsync(int tenantId)
        {
            var tenantNationalSpecification = new FindTenantNationalHolidayByTenantIdSpec(tenantId);

            var result = await Db.SingleAsync(tenantNationalSpecification.GetExpression());

            return result;
        }

        public async Task<bool> TenantNationalHolidayCheckAnyAsync(int yearIsCreated, int tenantId)
        {
            var tenantSpecification =
                new FindTenantNationalHolidayByLastCreatedYearSpec(yearIsCreated).And(
                    new FindTenantNationalHolidayByTenantIdSpec(tenantId));

            return await Db.ExistsAsync(tenantSpecification.GetExpression());
        }
    }
}
