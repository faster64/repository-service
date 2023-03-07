using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class TimeSheetShiftWriteOnlyRepository : EfBaseWriteOnlyRepository<TimeSheetShift>, ITimeSheetShiftWriteOnlyRepository
    {
        public TimeSheetShiftWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<TimeSheetShiftWriteOnlyRepository> logger)
            : base(db, authService, logger)
        {
        }


    }
}
