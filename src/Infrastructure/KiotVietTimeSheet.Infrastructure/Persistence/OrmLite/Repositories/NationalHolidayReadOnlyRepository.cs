using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class NationalHolidayReadOnlyRepository : OrmLiteRepository<NationalHoliday, int>, INationalHolidayReadOnlyRepository
    {
        public NationalHolidayReadOnlyRepository(
                IDbConnectionFactory db,
                IAuthService authService)
            : base(db, authService) { }

        public async Task<List<NationalHoliday>> NationHolidayGetAllAsync()
        {
            var spec = (new DefaultTrueSpec<NationalHoliday>()).GetExpression();
            return await Db.LoadSelectAsync(spec);
        }
    }
}
