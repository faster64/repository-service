using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using ServiceStack.OrmLite;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IClockingReadOnlyRepository : IBaseReadOnlyRepository<Clocking, long>
    {
        Task<PagingDataSource<ClockingDto>> FiltersAsync(ISqlExpression query);

        /// <summary>
        /// Lấy danh sách chi tiết ca làm việc trên Calendar
        /// </summary>
        /// <param name="query"></param>
        /// <param name="clockingHistoryStates"></param>
        /// <param name="departmentIds"></param>
        /// <returns></returns>
        Task<PagingDataSource<ClockingDto>> GetListClockingForCalendar(ISqlExpression query, List<byte> clockingHistoryStates, List<long> departmentIds);

        /// <summary>
        /// Lấy danh sách chi tiết ca làm việc trên Calendar
        /// </summary>
        /// <param name="branchIds"></param>
        /// <param name="clockingHistoryStates"></param>
        /// <param name="departmentIds"></param>
        /// <param name="shiftIds"></param>
        /// <param name="employeeIds"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="clockingStatusExtension"></param>
        /// <returns></returns>
        Task<PagingDataSource<ClockingDto>> GetClockingMultipleBranchForCalendars(List<int> branchIds, List<byte> clockingHistoryStates,
            List<long> departmentIds, List<long> shiftIds, List<long> employeeIds, DateTime startTime, DateTime endTime, List<byte> clockingStatusExtension);

        /// <summary>
        /// Lấy các chi tiết làm việc trùng thời gian với kỳ nghỉ
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        Task<List<Clocking>> GetClockingOverlapHoliday(DateTime from, DateTime to);

        /// <summary>
        /// Lấy các chi tiết làm việc cho bảng lương
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="employeeIds"></param>
        /// <returns></returns>
        Task<List<Clocking>> GetClockingForPaySheet(DateTime from, DateTime to, List<long> employeeIds);

        /// <summary>
        /// Lấy các chi tiết vi phạm cho bảng lương
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="employeeIds"></param>
        /// <returns></returns>
        Task<List<ClockingPenalizeDto>> GetClockingPenalizeForPaySheet(DateTime from, DateTime to, List<long> employeeIds);

        /// <summary>
        /// Hàm đếm  số ngày nghỉ không phép của nhân viên
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="employeeIds"></param>
        /// <returns></returns>
        Task<List<KeyValuePair<long, int>>> GetClockingUnAuthorizedAbsence(DateTime from, DateTime to,
            List<long> employeeIds);

        /// <summary>
        /// Hàm đếm  số ngày nghỉ không phép của nhân viên theo paysheetIds
        /// </summary>
        /// <param name="paySheetIds"></param>
        /// <param name="payslipIds"></param>
        /// <param name="employeeId"></param>
        /// /// <returns></returns>
        Task<List<KeyValuePair<long, int>>> GetClockingUnAuthorizeAbsenceByPaySheetIds(List<long> paySheetIds,
            List<long> payslipIds, long employeeId);

        /// <summary>
        /// Lấy danh sách chi tiết ca làm việc trên Mobile
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        Task<List<ClockingDto>> GetClockingsForClockingGps(int branchId, long employeeId);

        Task<List<Clocking>> GetClockingBySpecificationForClockingGpsAsync(ISpecification<Clocking> spec, bool includeSoftDelete = false);
    }
}
