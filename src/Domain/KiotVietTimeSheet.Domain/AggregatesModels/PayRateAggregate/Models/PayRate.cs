using System;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Events;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Factories;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using Newtonsoft.Json;
using ServiceStack;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models
{
    public class PayRate : BaseEntity,
         IAggregateRoot,
         IEntityIdlong,
         ITenantId,
         ICreatedBy,
         ICreatedDate,
         IModifiedBy,
         IModifiedDate
    {

        #region properties
        /// <summary>
        /// Id mức lương
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Id nhân viên có mức lương hiện tại
        /// </summary>
        public long EmployeeId { get; protected set; }

        /// <summary>
        /// Id loại lương mà nhân viên được gán
        /// </summary>
        public long? PayRateTemplateId { get; set; }

        /// <summary>
        /// Id doanh nghiệp/ cửa hàng
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// Thời điểm tạo
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Id người tạo
        /// </summary>
        public long CreatedBy { get; set; }

        /// <summary>
        /// Id người chỉnh sửa cuối cùng
        /// </summary>
        public long? ModifiedBy { get; set; }

        /// <summary>
        /// Thời điểm chỉnh sửa cuối cùng
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Chu kì trả lương <see cref="PaysheetAggregate.Enum.PaySheetWorkingPeriodStatuses"/>
        /// </summary>
        public byte SalaryPeriod { get; set; }

        public List<PayRateDetail> PayRateDetails { get; set; }
        #endregion

        [JsonConstructor]
        public PayRate(
            long id,
            long employeeId,
            int tenantId,
            long? payRateTemplateId,
            byte salaryPeriod,
            DateTime createdDate,
            long createdBy,
            long? modifiedBy,
            DateTime? modifiedDate
        )
        {
            Id = id;
            TenantId = tenantId;
            PayRateTemplateId = payRateTemplateId;
            EmployeeId = employeeId;
            SalaryPeriod = salaryPeriod;
            CreatedDate = createdDate;
            ModifiedBy = modifiedBy;
            ModifiedDate = modifiedDate;
            CreatedBy = createdBy;
        }

        // Only copy primitive values
        public PayRate(PayRate payRate
        )
        {
            Id = payRate.Id;
            TenantId = payRate.TenantId;
            PayRateTemplateId = payRate.PayRateTemplateId;
            EmployeeId = payRate.EmployeeId;
            SalaryPeriod = payRate.SalaryPeriod;
            CreatedDate = payRate.CreatedDate;
            ModifiedBy = payRate.ModifiedBy;
            ModifiedDate = payRate.ModifiedDate;
            CreatedBy = payRate.CreatedBy;

        }

        public PayRate(
            long employeeId,
            long? payRateTemplateId,
            byte salaryPeriod,
            List<IRule> rules
        )
        {
            EmployeeId = employeeId;
            SalaryPeriod = salaryPeriod;
            PayRateTemplateId = payRateTemplateId;
            PayRateDetails = new List<PayRateDetail>();
            rules.ForEach(r =>
            {
                var ruleEntity = r.ToEntity();
                AddOrUpdatePayRateDetail(ruleEntity.Type, ruleEntity.Value);
            });
            AddDomainEvent(new CreatedPayRateEvent(this));
        }

        public void AddOrUpdatePayRateDetail(string ruleType, string ruleValue)
        {
            if (PayRateDetails == null) PayRateDetails = new List<PayRateDetail>();

            var existsDetail = PayRateDetails.FirstOrDefault(pd => pd.RuleType == ruleType);
            if (existsDetail == null)
            {
                PayRateDetails.Add(new PayRateDetail(Id, ruleType, ruleValue, TenantId));
            }
            else
            {
                existsDetail.Update(ruleValue);
            }
        }

        public void Update(long? payRateTemplateId, byte salaryPeriod, List<IRule> rules)
        {
            var beforeChange = CopyBeforeChange();

            SalaryPeriod = salaryPeriod;
            PayRateTemplateId = payRateTemplateId;
            PayRateDetails = PayRateDetails ?? new List<PayRateDetail>();
            PayRateDetails = PayRateDetails.Where(pd => rules.Any(r => r.GetType().Name == pd.RuleType)).ToList();
            rules.ForEach(r =>
            {
                var ruleEntity = r.ToEntity();
                AddOrUpdatePayRateDetail(ruleEntity.Type, ruleEntity.Value);
            });

            AddDomainEvent(new UpdatedPayRateEvent(beforeChange, this));
        }

        public void Update(List<PayRateDetail> payRateDetails, byte salaryPeriod)
        {
            var beforeChange = CopyBeforeChange();
            PayRateDetails = payRateDetails;
            SalaryPeriod = salaryPeriod;
            AddDomainEvent(new UpdatedPayRateEvent(beforeChange, this));
        }

        public bool IsEqual(PayRate payRate)
        {
            if (payRate.SalaryPeriod != SalaryPeriod) return false;
            if (PayRateDetails == null || payRate.PayRateDetails == null) return true;
            if (PayRateDetails != null && payRate.PayRateDetails == null) return false;
            if (PayRateDetails == null && payRate.PayRateDetails != null) return false;
            if (PayRateDetails?.Count != payRate.PayRateDetails.Count) return false;

            var rules = PayRateDetails.Select(payRateDetail => RuleFactory.GetRule(payRateDetail.RuleType, payRateDetail.RuleValue)).ToList();
            var compareRules = payRate.PayRateDetails.Select(payRateDetail => RuleFactory.GetRule(payRateDetail.RuleType, payRateDetail.RuleValue)).ToList();

            return rules.TrueForAll(rule =>
            {
                var compareRule = compareRules.FirstOrDefault(r => r.GetType().Name == rule.GetType().Name);
                return rule == null && compareRule == null || (rule != null && rule.IsEqual(compareRule));
            });

        }

        private PayRate CopyBeforeChange()
        {
            var beforeChange = this.CreateCopy();
            beforeChange.PayRateDetails = PayRateDetails?.Select(x => new PayRateDetail(x.Id, x.TenantId, x.PayRateId, x.RuleType, x.RuleValue)).ToList();
            return beforeChange;
        }
    }
}
