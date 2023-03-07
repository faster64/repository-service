using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KiotVietTimeSheet.Application.DomainService.Impls
{
    public class PaySheetOutOfDateDomainService : IPaySheetOutOfDateDomainService
    {
        private readonly IPaysheetWriteOnlyRepository _paysheetWriteOnlyRepository;
        public PaySheetOutOfDateDomainService(IPaysheetWriteOnlyRepository paysheetWriteOnlyRepository)
        {
            _paysheetWriteOnlyRepository = paysheetWriteOnlyRepository;
        }

        public async Task WithPaysheetChangeAsync(List<long> paysheetIds)
        {
            var paysheets = await _paysheetWriteOnlyRepository.GetBySpecificationAsync(new FindPaysheetByIdsSpec(paysheetIds));
            paysheets.ForEach(p => p.Version += 1);
            _paysheetWriteOnlyRepository.BatchUpdate(paysheets);
        }
        public async Task WithPayRateDataChangeAsync(List<long> employeeIds)
        {
            var paysheets = await _paysheetWriteOnlyRepository.GetPaysheetDraftAndTempByEmployeeIds(employeeIds.Distinct().ToList());
            paysheets.ForEach(p => p.Version += 1);
            _paysheetWriteOnlyRepository.BatchUpdate(paysheets);
        }

        public async Task WithClockingDataChangeAsync(List<Clocking> clockings)
        {
            var paysheets = await _paysheetWriteOnlyRepository.GetPaysheetDraftAndTempByClockings(clockings);
            paysheets.ForEach(p => p.Version += 1);
            _paysheetWriteOnlyRepository.BatchUpdate(paysheets);
        }

        public async Task WithClockingDataChangeAsync(List<Clocking> clockings, long? withoutPaysheetId)
        {
            var paysheets = (await _paysheetWriteOnlyRepository.GetPaysheetDraftAndTempByClockings(clockings)).Where(p => withoutPaysheetId == null || p.Id != withoutPaysheetId).ToList();
            paysheets.ForEach(p => p.Version += 1);
            _paysheetWriteOnlyRepository.BatchUpdate(paysheets);
        }

        public async Task WithSettingsChangeAsync(int branchId)
        {
            var paysheets = await _paysheetWriteOnlyRepository.GetBySpecificationAsync(new FindPaysheetByBranchId(branchId));
            paysheets.ForEach(p => p.Version += 1);
            _paysheetWriteOnlyRepository.BatchUpdate(paysheets);
        }

        public async Task WithHolidayChangeAsync(DateTime startTime, DateTime endTime)
        {
            var paysheets = await _paysheetWriteOnlyRepository.GetBySpecificationAsync(new FindPaysheetByTimeRange(startTime, endTime));
            paysheets.ForEach(p => p.Version += 1);
            _paysheetWriteOnlyRepository.BatchUpdate(paysheets);
        }

        public async Task WithChangeBranchSettingAsync(int branchId)
        {
            var paySheets = await _paysheetWriteOnlyRepository.GetBySpecificationAsync(new FindPaysheetByBranchId(branchId));
            paySheets.ForEach(p => p.Version += 1);
            _paysheetWriteOnlyRepository.BatchUpdate(paySheets);
        }

        public async Task WithCommissionTableDataChangeAsync(List<long> commissionIds)
        {
            var paysheets = await _paysheetWriteOnlyRepository.GetPaysheetDraftAndTempByCommissionIds(commissionIds);
            paysheets.ForEach(p => p.Version += 1);
            _paysheetWriteOnlyRepository.BatchUpdate(paysheets);
        }

        public bool IsCheckPayRateDetail(PayRate payRate, List<Allowance> allowances, List<Deduction> deductions)
        {
            if (payRate?.PayRateDetails == null || !payRate.PayRateDetails.Any()) return false;

            // Kiểm tra trong trường hợp không thiết lập mức lương nào cho nhân viên và mặc định giảm trừ rỗng
            var deduction = payRate.PayRateDetails.Count == 1 &&
                            payRate.PayRateDetails[0].RuleType == typeof(DeductionRule).Name;
            if (deduction)
            {
                var deductionRuleValue = JsonConvert.DeserializeObject(payRate.PayRateDetails[0].RuleValue);
                var value = ((JProperty)((JContainer)deductionRuleValue).First).Value.First;
                if (value == null) return false;
            }

            // Kiểm tra nếu không có giảm trừ, phụ cấp sẽ trả về để xử lý tiếp
            var isDeductionOrAllowance = payRate.PayRateDetails.All(x =>
                x.RuleType == typeof(DeductionRule).Name || x.RuleType == typeof(AllowanceRule).Name);
            if (!isDeductionOrAllowance) return true;

            // Kiểm tra nếu tât cả giảm trừ, phụ cấp đã bị xóa thi k tính lương cho nhân viên này
            var isHaveAllowances = false;
            var isHaveDeductions = false;
            var allowanceRule = payRate.PayRateDetails.FirstOrDefault(x => x.RuleType == typeof(AllowanceRule).Name);
            if (allowanceRule != null)
            {
                var existingAllowanceParam = (AllowanceRuleValue)JsonConvert.DeserializeObject(allowanceRule.RuleValue, typeof(AllowanceRuleValue));
                if (existingAllowanceParam != null)
                    isHaveAllowances = existingAllowanceParam.AllowanceRuleValueDetails.Any(x =>
                        allowances.Select(a => a.Id).Contains(x.AllowanceId));
            }

            var deductionRule = payRate.PayRateDetails.FirstOrDefault(x => x.RuleType == typeof(DeductionRule).Name);
            if (deductionRule != null)
            {
                var existingDeductionParam = (DeductionRuleValue)JsonConvert.DeserializeObject(deductionRule.RuleValue, typeof(DeductionRuleValue));
                if (existingDeductionParam != null)
                    isHaveDeductions = existingDeductionParam.DeductionRuleValueDetails.Any(x =>
                        deductions.Select(a => a.Id).Contains(x.DeductionId));
            }

            return (isHaveAllowances || isHaveDeductions);
        }
    }
}
