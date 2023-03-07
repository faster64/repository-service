using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;

namespace KiotVietTimeSheet.Application.DomainService.Interfaces
{
    public interface ICompleteSalaryClockingDomainService
    {
        Task CompletePaysheetForClockingsAsync(Paysheet paysheet);
    }
}
