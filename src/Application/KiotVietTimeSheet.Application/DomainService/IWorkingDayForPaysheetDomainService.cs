using System;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.DomainService
{
    public interface IWorkingDayForPaysheetDomainService
    {
        Task<int> GetWorkingDayPaysheetAsync(int branchId, DateTime from, DateTime to);
    }
}
