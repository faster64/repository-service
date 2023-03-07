using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.OvertimeSalary;

namespace KiotVietTimeSheet.Application.Dto
{
    public class PayslipDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long PaysheetId { get; set; }
        public int TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public string Note { get; set; }
        public byte PayslipStatus { get; set; }
        public long EmployeeId { get; set; }
        public PaysheetDto Paysheet { get; set; }
        public EmployeeDto Employee { get; set; }
        public decimal MainSalary { get; set; }
        public decimal CommissionSalary { get; set; }
        public decimal OvertimeSalary { get; set; }
        public decimal Allowance { get; set; }
        public decimal Deduction { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// Tiền đã trả trước cho nhân viên (bao gồm tạm ứng, tiền trả thừa ...)
        /// </summary>
        public decimal TotalPayment { get; set; }


        private decimal? _totalNeedPay;
        /// <summary>
        /// Còn cần trả
        /// </summary>
        public decimal TotalNeedPay
        {
            get
            {
                if (_totalNeedPay.HasValue) return _totalNeedPay.Value;
                _totalNeedPay = NetSalary - TotalPayment;
                _totalNeedPay = _totalNeedPay > 0 ? _totalNeedPay : 0;
                return _totalNeedPay.Value;
            }

        }
        public decimal Bonus { get; set; }
        public MainSalaryRuleParam MainSalaryRuleParam { get; set; }
        public CommissionSalaryRuleParamV2 CommissionSalaryRuleParam { get; set; }
        public OvertimeSalaryRuleParam OvertimeSalaryRuleParam { get; set; }
        public AllowanceRuleParam AllowanceRuleParam { get; set; }
        public DeductionRuleParam DeductionRuleParam { get; set; }
        public MainSalaryRuleValue MainSalaryRuleValue { get; set; }
        public CommissionSalaryRuleValueV2 CommissionSalaryRuleValue { get; set; }
        public OvertimeSalaryRuleValue OvertimeSalaryRuleValue { get; set; }
        public AllowanceRuleValue AllowanceRuleValue { get; set; }
        public DeductionRuleValue DeductionRuleValue { get; set; }
        public decimal NetSalary { get; set; }
        public decimal GrossSalary { get; set; }
        public List<PayslipPaymentDto> PayslipPayments { get; set; }
        public List<PayslipClockingDto> PayslipClockings { get; set; }
        public List<PayslipDetailDto> PayslipDetails { get; set; }
        public List<PayslipClockingPenalizeDto> PayslipClockingPenalizes { get; set; }
        public List<PayslipPenalizeDto> PayslipPenalizes { get; set; }
        public DateTime? PayslipCreatedDate { get; set; }
        public long? PayslipCreatedBy { get; set; }
        public decimal AuthorisedAbsence { get; set; }
        public int UnauthorisedAbsence { get; set; }
        public string ContentErr { get; protected set; }

    }
}
