using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.WriteRepositories
{
    public interface ITimeSheetWriteOnlyRepository : IBaseWriteOnlyRepository<TimeSheet>
    {
        List<TimeSheet> GetTimeSheetForBranchSetting(int branchId, DateTime applyFrom);
        Task<List<TimeSheet>> FindByIdWithoutPermission(long employeeId);
    }
}
