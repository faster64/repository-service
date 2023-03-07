using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.DomainService.Interfaces
{
    public interface IDetectionChangePaysheetDomainService
    {
        Task<bool> IsChangePaysheetWhenMakePaymentsAsync(long paysheetId, long paysheetVersion);
    }
}
