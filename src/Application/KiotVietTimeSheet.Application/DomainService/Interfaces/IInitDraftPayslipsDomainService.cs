using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;

namespace KiotVietTimeSheet.Application.DomainService.Interfaces
{
    public interface IInitDraftPayslipsDomainService
    {
        /// <summary>
        /// Khởi tạo Danh sách phiếu lương nháp
        /// </summary>
        /// <param name="payslips"></param>
        /// <param name="employeeIds"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="salaryPeriod"></param>
        /// <param name="standardWorkingDayNumber"></param>
        /// <param name="timeOfStandardWorkingDay"></param>
        /// <param name="branchId"></param>
        /// <param name="payslipCreatedBy"></param>
        /// <param name="payslipCreatedDate"></param>
        /// <returns></returns>
        Task<List<DraftPayslipDomainServiceDto>> InitDraftPayslipsAsync(List<Payslip> payslips, List<long> employeeIds, DateTime from, DateTime to,
            byte salaryPeriod, int standardWorkingDayNumber, int timeOfStandardWorkingDay, int branchId, long? payslipCreatedBy = null,
            DateTime? payslipCreatedDate = null, SettingsToObject settings = null);
    }
}
