using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class ClockingWriteOnlyRepository : EfBaseWriteOnlyRepository<Clocking>, IClockingWriteOnlyRepository
    {
        public ClockingWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<ClockingWriteOnlyRepository> logger) : base(db, authService, logger)
        {

        }

        public async Task<List<Clocking>> GetClockingsForPaysheet(DateTime from, DateTime to, List<long> employeeIds)
        {
            var clockings = await Db.Clockings.Where(c =>
                    c.ClockingStatus == (byte)ClockingStatuses.CheckedOut ||
                    (c.ClockingStatus == (byte)ClockingStatuses.WorkOff &&
                     c.AbsenceType == (byte)AbsenceTypes.AuthorisedAbsence))
                .Where(c => c.StartTime >= from)
                .Where(c => c.StartTime < to.Date.AddDays(1))
                .Where(c => c.ClockingPaymentStatus == (byte)ClockingPaymentStatuses.UnPaid)
                .Where(c => employeeIds.Contains(c.EmployeeId))
                .Where(c => c.TenantId == AuthService.Context.TenantId && !c.IsDeleted)
                .Include(x => x.ClockingPenalizes)
                .ToListAsync();

            return clockings;
        }

        public async Task<List<long>> GetListEmployIdWorking(DateTime from, DateTime to, List<long> employeeIds)
        {
            var query = Db.Clockings.Where(x => 
                x.TenantId == AuthService.Context.TenantId &&
                !x.IsDeleted &&
                x.ClockingStatus != (byte) ClockingStatuses.Void &&
                (x.CheckInDate >= from || x.CheckOutDate >= from) &&
                (x.CheckInDate <= to || x.CheckOutDate <= to));

            if (employeeIds?.Any() == true)
            {
                query = query.Where(x => employeeIds.Contains(x.EmployeeId));
            }

            var result = await query.Select(x => x.EmployeeId).ToListAsync();

            return result;
        }

        public async Task<long> UpdateClockingTimeByShift(int tenantId, int branchId, long shifId, long shiftFrom, long shiftTo)
        {
            var returnValue = new SqlParameter("@returnValue", SqlDbType.BigInt)
            {
                Direction = ParameterDirection.Output
            };
            var param = new List<SqlParameter> {
                new SqlParameter("@tenantId", tenantId),
                new SqlParameter("@branchId", branchId),
                new SqlParameter("@shiftId", shifId),
                new SqlParameter("@shiftFrom", shiftFrom),
                new SqlParameter("@shiftTo", shiftTo),
                returnValue
            };
            await Db.Database.ExecuteSqlCommandAsync("pr_Update_Clocking_Time @tenantId, @branchId, @shiftId, @shiftFrom, @shiftTo, @returnValue OUTPUT", param);
            return Convert.ToInt64(returnValue.Value);
        }

        public async Task<List<Clocking>> FindByIdAndStatusWithoutPermission(long employeeId, byte status)
        {
            return await Db.Clockings.Where(p => p.TenantId == AuthService.Context.TenantId && p.EmployeeId == employeeId && p.ClockingStatus == status)
                .ToListAsync();
        }
    }
}