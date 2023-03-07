using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Dto;

namespace KiotVietTimeSheet.Application.DomainService
{
    public interface IGenerateClockingsDomainService
    {
        /// <summary>
        /// Tạo nhiều chi tiết làm việc cho một hoặc nhiều lịch làm việc
        /// </summary>
        /// <param name="generateClockingForTimeSheetsDto"></param>
        /// <returns></returns>
        Task<GenerateClockingsDomainServiceDto> GenerateClockingForTimeSheets(GenerateClockingForTimeSheetsDto generateClockingForTimeSheetsDto);
    }
}
