using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Repositories.WriteRepositories
{
    public interface IAutoKeepingWriteOnlyRepository
    {
        Task<List<Clocking>> AutoKeepingAsync(int tenantId, DateTime startTime, DateTime endTime,Guid autoTimekeepingUid, long jobId);        
    }
}