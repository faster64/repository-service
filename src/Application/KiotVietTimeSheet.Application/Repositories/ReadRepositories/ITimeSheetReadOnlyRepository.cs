using System;
using KiotVietTimeSheet.Application.Dto;
using System.Threading.Tasks;
using ServiceStack.OrmLite;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using System.Collections.Generic;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface ITimeSheetReadOnlyRepository : IBaseReadOnlyRepository<TimeSheet, long>
    {
        Task<PagingDataSource<TimeSheetDto>> FiltersAsync(ISqlExpression query);

        /// <summary>
        /// lấy ra các lịch làm việc có data giống với các lịch làm việc đưa vào, dùng để validate những lịch làm việc giống nhau
        /// </summary>
        /// <param name="timeSheets"></param>
        /// <returns></returns>
        Task<List<TimeSheet>> GetTimeSheetByTimeSheets(List<TimeSheet> timeSheets);

        /// <summary>
        /// Lấy các lịch làm việc trùng thời gian với kỳ nghỉ
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        Task<List<TimeSheet>> GetTimeSheetOverlayHoliday(DateTime from, DateTime to);
    }
}
