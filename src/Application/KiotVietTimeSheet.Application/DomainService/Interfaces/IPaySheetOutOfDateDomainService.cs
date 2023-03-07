using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;

namespace KiotVietTimeSheet.Application.DomainService.Interfaces
{
    public interface IPaySheetOutOfDateDomainService
    {
        /// <summary>
        ///  chuyển trạng thái out of date cho bảng lương 
        /// </summary>
        /// <param name="paysheetIds"></param>
        /// <returns></returns>
        Task WithPaysheetChangeAsync(List<long> paysheetIds);
        /// <summary>
        /// chuyển trạng thái out of date cho bảng lương liên quan khi có sự thay đổi mức lương của danh sách nhân viên
        /// </summary>
        /// <param name="employeeIds">Id của nhân viên có sự thay đổi dữ liệu</param>
        Task WithPayRateDataChangeAsync(List<long> employeeIds);

        /// <summary>
        /// chuyển trạng thái out of date cho bảng lương liên quan khi có sự thay đổi dữ liệu chấm công
        /// </summary>
        /// <param name="clockings"></param>
        /// <returns></returns>
        Task WithClockingDataChangeAsync(List<Clocking> clockings);

        /// <summary>
        /// chuyển trạng thái out of date cho bảng lương liên quan khi có sự thay đổi dữ liệu chấm công
        /// </summary>
        /// <param name="clockings"></param>
        /// <returns></returns>
        Task WithClockingDataChangeAsync(List<Clocking> clockings, long? withoutPaysheetId);

        /// <summary>
        /// Chuyển trạng thái out of date cho bảng lương liên quan khi có sự thay đổi cài đặt chi nhánh
        /// </summary>
        Task WithSettingsChangeAsync(int branchId);

        /// <summary>
        /// Chuyển trạng thái out of date cho bảng lương liên quan khi có sự thay đổi ngày nghỉ lễ tết
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        Task WithHolidayChangeAsync(DateTime startTime, DateTime endTime);

        /// <summary>
        /// Chuyển trạng thái out of date cho bảng lương liên quan khi có sự thay đổi ngày nghỉ chi nhánh
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        Task WithChangeBranchSettingAsync(int branchId);

        Task WithCommissionTableDataChangeAsync(List<long> commissionIds);

        bool IsCheckPayRateDetail(PayRate payRate, List<Allowance> allowances, List<Deduction> deductions);
    }
}
