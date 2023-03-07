using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.DomainService
{
    public interface ICreateTimeSheetClockingDomainService
    {
        Task<TimeSheetDomainServiceDto> CreateAsync(TimeSheetDto timeSheetDto);
        Task<TimeSheetDomainServiceDto> CreateWhenReplaceEmployeeAsync(TimeSheetDto timeSheetDto);
    }
}
