using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using Microsoft.Extensions.Logging;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class TimeSheetWriteOnlyRepository : EfBaseWriteOnlyRepository<TimeSheet>, ITimeSheetWriteOnlyRepository
    {
        private readonly IAuthService _authService;
        public TimeSheetWriteOnlyRepository(EfDbContext db,
            IAuthService authService,
            ILogger<TimeSheetWriteOnlyRepository> logger
        )
            : base(db, authService, logger)
        {
            _authService = authService;
            
        }
        public List<TimeSheet> GetTimeSheetForBranchSetting(int branchId, DateTime applyFrom)
        {
            var timeSheets = (from timeSheet in Db.Set<TimeSheet>().Include(t => t.TimeSheetShifts).AsNoTracking()
                              join emp in Db.Set<Employee>() on timeSheet.EmployeeId equals emp.Id
                              join tss in Db.Set<TimeSheetShift>() on timeSheet.Id equals tss.TimeSheetId

                              where
                                  // bỏ qua các lịch làm việc đã hủy hoắc có nhân viên ngừng hoạt động
                                  !emp.IsDeleted
                                  && timeSheet.TenantId == _authService.Context.TenantId &&
                                  timeSheet.BranchId == branchId
                                  && !(timeSheet as ISoftDelete).IsDeleted &&
                                  timeSheet.TimeSheetStatus != (byte)TimeSheetStatuses.Void &&
                                  timeSheet.EndDate >= applyFrom
                              select timeSheet).Distinct().ToList();
            return timeSheets;


        }

        public async Task<List<TimeSheet>> FindByIdWithoutPermission(long employeeId)
        {
            return await Db.TimeSheets.Where(p => p.TenantId == AuthService.Context.TenantId && p.EmployeeId == employeeId)
                .ToListAsync();
        }
    }
}
