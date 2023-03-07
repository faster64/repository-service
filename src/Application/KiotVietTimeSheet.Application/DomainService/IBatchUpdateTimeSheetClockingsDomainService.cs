using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;

namespace KiotVietTimeSheet.Application.DomainService
{
    public interface IBatchUpdateTimeSheetClockingsDomainService
    {
        /// <summary>
        /// Cập nhật danh sách chi tiết làm việc của các lịch làm việc
        /// </summary>
        /// <param name="generateClockingForTimeSheetsDto"></param>
        /// <returns></returns>
        Task<TimeSheetDomainServiceDto> BatchUpdateTimeSheetClockingWhenUpdateDaysOffAsync(GenerateClockingForTimeSheetsDto generateClockingForTimeSheetsDto);

        Task BatchUpdateTimeSheetAndClocking(List<TimeSheet> timeSheets, List<Clocking> timeSheetClockings,
            List<long> clockingUpdateIds, (TimeSheetDto, TimeSheet, bool) values);

        /// <summary>
        /// Cập nhật lịch làm việc và chi tiết làm việc khi thay đổi ngày nghỉ
        /// </summary>
        /// <param name="timeSheets"></param>
        /// <param name="timeSheetClockings"></param>
        /// <param name="clockingUpdateIds"></param>
        /// <returns></returns>
        Task BatchUpdateTimeSheetAndClockingWhenChangeDayOffAsync(List<TimeSheet> timeSheets, List<Clocking> timeSheetClockings,
             List<long> clockingUpdateIds);
    }
}
