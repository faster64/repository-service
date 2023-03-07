using System;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Events;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Objects;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.OvertimeSalary;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models
{
    public class Payslip : BaseEntity,
        IAggregateRoot,
        IEntityIdlong,
        ICode,
        ITenantId,
        ISoftDelete,
        ICreatedDate,
        ICreatedBy,
        IModifiedBy,
        IModifiedDate,
        IIsDraft
    {

        #region Properties
        public static string CodePrefix = "PL"; //NOSONAR
        public static string CodeDelSuffix = "{DEL"; //NOSONAR
        public static string CodeDraftSuffix = "{Draft}"; //NOSONAR

        public long Id { get; set; }
        public string Code { get; set; }
        public long PaysheetId { get; protected set; }
        public int TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string Note { get; protected set; }
        /// <summary>
        /// Trạng thái phiếu lương <see cref="PayslipStatuses"/>
        /// </summary>
        public byte PayslipStatus { get; protected set; }
        public long EmployeeId { get; protected set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<PayslipDetail> PayslipDetails { get; set; }
        public List<PayslipClocking> PayslipClockings { get; set; }
        public List<PayslipClockingPenalize> PayslipClockingPenalizes { get; set; }
        public List<PayslipPenalize> PayslipPenalizes { get; set; }
        /// <summary>
        /// Lương chính
        /// </summary>
        public decimal MainSalary { get; protected set; }

        /// <summary>
        /// Lương kinh doanh
        /// </summary>
        public decimal CommissionSalary { get; protected set; }

        /// <summary>
        /// Lương làm thêm
        /// </summary>
        public decimal OvertimeSalary { get; protected set; }

        /// <summary>
        /// Phụ cấp
        /// </summary>
        public decimal Allowance { get; protected set; }

        /// <summary>
        /// Giảm trừ
        /// </summary>
        public decimal Deduction { get; protected set; }

        /// <summary>
        /// Thưởng
        /// </summary>
        public decimal Bonus { get; protected set; }

        /// <summary>
        /// Thực lĩnh hoặc Tổng lương
        /// </summary>
        public decimal NetSalary { get; protected set; }

        /// <summary>
        /// Tổng thu nhập
        /// </summary>
        public decimal GrossSalary { get; protected set; }

        /// <summary>
        /// Tổng tiền đã thanh toán
        /// </summary>
        public decimal TotalPayment { get; protected set; }

        /// <summary>
        /// Cờ phiếu lương nháp của hệ thống
        /// </summary>
        public bool IsDraft { get; set; }

        public DateTime? PayslipCreatedDate { get; protected set; }
        public long? PayslipCreatedBy { get; protected set; }
        #endregion

        #region Constructors
        [JsonConstructor]
        public Payslip(
            long id,
            string code,
            long paysheetId,
            int tenantId,
            bool isDeleted,
            long? deletedBy,
            DateTime? deletedDate,
            string note,
            byte payslipStatus,
            long employeeId,
            DateTime createdDate,
            long createdBy,
            long? modifiedBy,
            DateTime? modifiedDate,
            decimal mainSalary,
            decimal commissionSalary,
            decimal overtimeSalary,
            decimal allowance,
            decimal deduction,
            decimal bonus,
            decimal netSalary
        )
        {
            Id = id;
            Code = code;
            PaysheetId = paysheetId;
            TenantId = tenantId;
            IsDeleted = isDeleted;
            DeletedBy = deletedBy;
            DeletedDate = deletedDate;
            Note = note;
            PayslipStatus = payslipStatus;
            EmployeeId = employeeId;
            CreatedDate = createdDate;
            CreatedBy = createdBy;
            ModifiedBy = modifiedBy;
            ModifiedDate = modifiedDate;
            MainSalary = mainSalary;
            CommissionSalary = commissionSalary;
            OvertimeSalary = overtimeSalary;
            Allowance = allowance;
            Deduction = deduction;
            Bonus = bonus;
            NetSalary = netSalary;
        }

        // Only copy primitive values
        public Payslip(Payslip payslip)
        {
            Id = payslip.Id;
            Code = payslip.Code;
            PaysheetId = payslip.PaysheetId;
            TenantId = payslip.TenantId;
            IsDeleted = payslip.IsDeleted;
            DeletedBy = payslip.DeletedBy;
            DeletedDate = payslip.DeletedDate;
            Note = payslip.Note;
            PayslipStatus = payslip.PayslipStatus;
            EmployeeId = payslip.EmployeeId;
            CreatedDate = payslip.CreatedDate;
            CreatedBy = payslip.CreatedBy;
            ModifiedBy = payslip.ModifiedBy;
            ModifiedDate = payslip.ModifiedDate;
            MainSalary = payslip.MainSalary;
            CommissionSalary = payslip.CommissionSalary;
            OvertimeSalary = payslip.OvertimeSalary;
            Allowance = payslip.Allowance;
            Deduction = payslip.Deduction;
            Bonus = payslip.Bonus;
            NetSalary = payslip.NetSalary;
            GrossSalary = payslip.GrossSalary;

            if (payslip.DomainEvents != null)
            {
                foreach (var domainEvent in payslip.DomainEvents)
                {
                    AddDomainEvent(domainEvent);
                }
            }
        }

        public Payslip(List<IRule> rules, long employeeId, byte payslipStatus, EngineResource engineResource, long? payslipCreatedBy, DateTime? payslipCreatedDate, bool isDraft)
        {
            PayslipDetails = new List<PayslipDetail>();
            EmployeeId = employeeId;
            Bonus = 0;
            PayslipStatus = payslipStatus;
            IsDraft = isDraft;
            PayslipCreatedBy = payslipCreatedBy;
            PayslipCreatedDate = payslipCreatedDate;

            decimal totalMoneyClockingPenalize = 0;
            if (engineResource.ClockingPenalizes != null)
            {
                totalMoneyClockingPenalize = CalculateTotalMoneyClockingPenalize(engineResource.ClockingPenalizes);
            }

            rules.ForEach(rule =>
            {

                if (rule == null) return;
                rule.Factory(engineResource);
                rule.Init();
                engineResource.Rules = rules;

                var ruleTypeItem = rule.GetType();
                var ruleEntity = rule.ToEntity();
                AddOrUpdatePayslipDetail(ruleEntity.Type, ruleEntity.Value, ruleEntity.Param);
                if (ruleTypeItem == typeof(MainSalaryRule))
                {
                    MainSalary = rule.Process();
                }
                else if (ruleTypeItem == typeof(AllowanceRule))
                {
                    Allowance = rule.Process();
                }
                else if (ruleTypeItem == typeof(DeductionRule))
                {
                    Deduction = rule.Process() + totalMoneyClockingPenalize;
                }
                else if (ruleTypeItem == typeof(OvertimeSalaryRule))
                {
                    OvertimeSalary = rule.Process();
                }
                else if (ruleTypeItem == typeof(CommissionSalaryRuleV2))
                {
                    CommissionSalary = rule.Process();
                }
            });

            NetSalary = new SalaryEngine(rules, 0, totalMoneyClockingPenalize).CalculateNetSalary();
            GrossSalary = new SalaryEngine(rules).CalculateGrossSalary();
        }

        public Payslip(
            long employeeId,
            long paysheetId,
            decimal mainSalary,
            decimal allowance,
            decimal deduction,
            decimal commissionSalary,
            decimal overtimeSalary,
            decimal bonus,
            byte payslipStatus,
            string code,
            string note,
            long? payslipCreatedBy,
            DateTime? payslipCreatedDate,
            List<IRule> rules
            )
        {
            EmployeeId = employeeId;
            PaysheetId = paysheetId;
            MainSalary = mainSalary;
            Allowance = allowance;
            Deduction = deduction;
            CommissionSalary = commissionSalary;
            OvertimeSalary = overtimeSalary;
            Bonus = bonus;
            PayslipStatus = payslipStatus;
            Code = code;
            Note = note;
            PayslipCreatedBy = payslipCreatedBy;
            PayslipCreatedDate = payslipCreatedDate;

            rules.ForEach(rule =>
            {
                if (rule == null) return;
                var ruleEntity = rule.ToEntity();
                AddOrUpdatePayslipDetail(ruleEntity.Type, ruleEntity.Value, ruleEntity.Param);
            });
            NetSalary = new SalaryEngine(mainSalary, allowance, deduction, commissionSalary, overtimeSalary, bonus).CalculateNetSalary();
            GrossSalary = new SalaryEngine(mainSalary, allowance, deduction, commissionSalary, overtimeSalary, bonus).CalculateGrossSalary();
        }
        #endregion

        #region Public Methods
        public void Update(
            decimal mainSalary,
            decimal allowance,
            decimal deduction,
            decimal commissionSalary,
            decimal overtimeSalary,
            decimal bonus,
            byte payslipStatus,
            string code,
            string note,
            long? payslipCreatedBy,
            DateTime? payslipCreatedDate,
            List<IRule> rules)
        {
            MainSalary = mainSalary;
            Allowance = allowance;
            Deduction = deduction;
            CommissionSalary = commissionSalary;
            OvertimeSalary = overtimeSalary;
            Bonus = bonus;
            PayslipStatus = payslipStatus;
            Code = code;
            Note = note;
            IsDraft = false;
            PayslipCreatedBy = payslipCreatedBy;
            PayslipCreatedDate = payslipCreatedDate;

            rules.ForEach(rule =>
            {
                if (rule == null) return;
                var ruleEntity = rule.ToEntity();
                AddOrUpdatePayslipDetail(ruleEntity.Type, ruleEntity.Value, ruleEntity.Param);
            });
            NetSalary = new SalaryEngine(mainSalary, allowance, deduction, commissionSalary, overtimeSalary, bonus).CalculateNetSalary();
            GrossSalary = new SalaryEngine(mainSalary, allowance, deduction, commissionSalary, overtimeSalary, bonus).CalculateGrossSalary();
        }

        public void UpdateWhenDataChanged(List<IRule> rules, EngineResource engineResource)
        {
            if (PayslipDetails != null && PayslipDetails.Any())
            {
                PayslipDetails.RemoveAll(p => !rules.Select(r => r.GetType().Name).ToList().Contains(p.RuleType));
            }

            var totalMoneyClockingPenalize = CalculateTotalMoneyClockingPenalize(engineResource.ClockingPenalizes);

            var typeRules = new List<Type>();
            rules.ForEach(rule =>
            {
                if (rule == null) return;
                rule.Factory(engineResource);
                rule.Init();
                engineResource.Rules = rules;

                var ruleType = rule.GetType();
                var existingRuleParam = PayslipDetails?.FirstOrDefault(p => p.RuleType == ruleType.Name)?.RuleParam;
                rule.UpdateParam(existingRuleParam);
                var ruleEntity = rule.ToEntity();
                typeRules.Add(ruleType);
                AddOrUpdatePayslipDetail(ruleEntity.Type, ruleEntity.Value, ruleEntity.Param);

                if (ruleType == typeof(MainSalaryRule))
                {
                    MainSalary = rule.Process();
                }
                else if (ruleType == typeof(AllowanceRule))
                {
                    Allowance = rule.Process();
                }
                else if (ruleType == typeof(DeductionRule))
                {
                    Deduction = rule.Process() + totalMoneyClockingPenalize;
                }
                else if (ruleType == typeof(OvertimeSalaryRule))
                {
                    OvertimeSalary = rule.Process();
                }
                else if (ruleType == typeof(CommissionSalaryRuleV2))
                {
                    CommissionSalary = rule.Process();
                }
            });

            MainSalary = typeRules.Contains(typeof(MainSalaryRule)) ? MainSalary : 0;
            Allowance = typeRules.Contains(typeof(AllowanceRule)) ? Allowance : 0;
            Deduction = typeRules.Contains(typeof(DeductionRule)) ? Deduction : 0;
            OvertimeSalary = typeRules.Contains(typeof(OvertimeSalaryRule)) ? OvertimeSalary : 0;
            CommissionSalary = typeRules.Contains(typeof(CommissionSalaryRuleV2)) ? CommissionSalary : 0;

            NetSalary = new SalaryEngine(rules, engineResource.Bonus, totalMoneyClockingPenalize).CalculateNetSalary();
            GrossSalary = new SalaryEngine(rules, engineResource.Bonus).CalculateGrossSalary();
        }

        private decimal CalculateTotalMoneyClockingPenalize(List<ClockingPenalize> clockingPenalizes)
        {
            decimal totalMoneyClockingPenalize = 0;
            if (clockingPenalizes != null)
            {
                totalMoneyClockingPenalize =
                    (from cp in clockingPenalizes
                     group cp by cp.PenalizeId
                    into grCp
                     select new
                     {
                         PenalizeId = grCp.Key,
                         TotalMoneyWithPenalizeId = grCp.Select(x => x.TimesCount * x.Value).Sum()
                     }).Sum(x => x.TotalMoneyWithPenalizeId);
            }

            return totalMoneyClockingPenalize;
        }

        public void Cancel()
        {
            if (IsDraft)
            {
                IsDeleted = true;
            }
            else
            {
                var oldPayslip = (Payslip)MemberwiseClone();
                PayslipStatus = (byte)PayslipStatuses.Void;
                PayslipClockings = new List<PayslipClocking>();
                AddDomainEvent(new CancelPayslipEvent(oldPayslip));
            }

        }

        public void CancelWithoutEvent()
        {
            PayslipStatus = (byte)PayslipStatuses.Void;
            PayslipClockings = new List<PayslipClocking>();
        }

        public void UpdatePaysheetId(long paysheetId)
        {
            PaysheetId = paysheetId;
        }

        public void Complete()
        {
            if (PayslipStatus != (byte)PayslipStatuses.Void)
                PayslipStatus = (byte)PayslipStatuses.PaidSalary;
        }

        public void UpdateTotalPaymentWithAmout(decimal amount)
        {
            TotalPayment += amount;
        }

        public void UpdateTotalPayment(decimal amount)
        {
            TotalPayment = amount;
        }

        public void UpdatePayslipStatus(byte payslipStatus)
        {
            PayslipStatus = payslipStatus;
        }

        #endregion

        #region Private methods
        public void AddOrUpdatePayslipDetail(string ruleType, string ruleValue, string ruleParam)
        {
            if (PayslipDetails == null) PayslipDetails = new List<PayslipDetail>();

            var existingDetail = PayslipDetails.FirstOrDefault(pd => pd.RuleType == ruleType);
            if (existingDetail == null)
            {
                PayslipDetails.Add(new PayslipDetail(Id, ruleType, ruleValue, ruleParam));
            }
            else
            {
                existingDetail.UpdateRuleValue(ruleValue);
                existingDetail.UpdateRuleParam(ruleParam);
            }
        }
        #endregion 
    }
}
