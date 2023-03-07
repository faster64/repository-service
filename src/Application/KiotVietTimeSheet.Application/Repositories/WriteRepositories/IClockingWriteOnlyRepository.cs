using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Repositories.WriteRepositories
{
    public interface IClockingWriteOnlyRepository : IBaseWriteOnlyRepository<Clocking>
    {
        Task<List<Clocking>> GetClockingsForPaysheet(DateTime from, DateTime to, List<long> employeeIds);

        Task<List<long>> GetListEmployIdWorking(DateTime from, DateTime to, List<long> employeeIds);

        Task<long> UpdateClockingTimeByShift(int tenantId, int branchId, long shifId, long shiftFrom, long shiftTo);
        Task<List<Clocking>> FindByIdAndStatusWithoutPermission(long employeeId, byte status);
    }
}