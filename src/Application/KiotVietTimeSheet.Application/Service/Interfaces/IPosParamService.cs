using System.Threading.Tasks;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Service.Interfaces
{
    public interface IPosParamService
    {
        Task<TimeSheetPosParam> GetTimeSheetPosParam(int retailerId, string tenantCode);
    }
}
