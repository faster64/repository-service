using System;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Events;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models
{
    public class PayRateTemplate : BaseEntity,
         IAggregateRoot,
         IEntityIdlong,
         ITenantId,
         IBranchId,
         ICreatedBy,
         ICreatedDate,
         IModifiedBy,
         IModifiedDate,
         IName,
         ISoftDeleteV2
    {
        public static PayRateTemplate Instance { get; } = new PayRateTemplate();

        // For EF
        private PayRateTemplate() { }

        /// <summary>
        /// Id mức lương
        /// </summary>
        public long Id { get; set; }


        /// <summary>
        /// Id doanh nghiệp/ cửa hàng
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// Tên loại lương
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Chu kì trả lương <see cref="PaysheetAggregate.Enum.PaySheetWorkingPeriodStatuses"/>
        /// </summary>
        public byte SalaryPeriod { get; set; }

        /// <summary>
        /// Chi nhánh làm việc
        /// </summary>
        public int BranchId { get; set; }

        /// <summary>
        /// Chi tiết mẫu lương
        /// </summary>
        public List<PayRateTemplateDetail> PayRateTemplateDetails { get; set; }

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

        [JsonConstructor]
        public PayRateTemplate(
            long id,
            int tenantId,
            string name,
            byte salaryPeriod,
            int branchId,
            List<PayRateTemplateDetail> payRateTemplateDetails,
            DateTime createdDate,
            long createdBy,
            long? modifiedBy,
            DateTime? modifiedDate
        )
        {
            Id = id;
            TenantId = tenantId;
            Name = name;
            SalaryPeriod = salaryPeriod;
            BranchId = branchId;
            PayRateTemplateDetails = payRateTemplateDetails;
            CreatedDate = createdDate;
            ModifiedBy = modifiedBy;
            ModifiedDate = modifiedDate;
            CreatedBy = createdBy;
        }

        public PayRateTemplate(string name, byte salaryPeriod, List<IRule> rules, int branchId, bool isGeneralSetting = false)
        {
            Name = name;
            SalaryPeriod = salaryPeriod;
            PayRateTemplateDetails = new List<PayRateTemplateDetail>();
            BranchId = branchId;
            rules.ForEach(r =>
            {
                var ruleEntity = r.ToEntity();
                AddOrUpdateRuleTemplateDetail(ruleEntity.Type, ruleEntity.Value);
            });

            AddDomainEvent(new CreatedPayRateTemplateEvent(this, isGeneralSetting));
        }

        public void Update(string name, byte salaryPeriod, List<IRule> rules, bool updatePayRate, bool isGeneralSetting = false)
        {
            var beforeChange = MemberwiseClone() as PayRateTemplate;
            PayRateTemplateDetails = PayRateTemplateDetails ?? new List<PayRateTemplateDetail>();
            if (beforeChange != null)
                beforeChange.PayRateTemplateDetails = PayRateTemplateDetails
                    .Select(x => new PayRateTemplateDetail(x.PayRateTemplateId, x.RuleType, x.RuleValue)).ToList();

            Name = name;
            SalaryPeriod = salaryPeriod;
            PayRateTemplateDetails = PayRateTemplateDetails ?? new List<PayRateTemplateDetail>();
            PayRateTemplateDetails = PayRateTemplateDetails.Where(pd => rules.Any(r => r.GetType().Name == pd.RuleType)).ToList();
            rules.ForEach(r =>
            {
                var ruleEntity = r.ToEntity();
                AddOrUpdateRuleTemplateDetail(ruleEntity.Type, ruleEntity.Value);
            });

            AddDomainEvent(new UpdatedPayRateTemplateEvent(beforeChange, this, updatePayRate, isGeneralSetting));
        }

        public void Copy(string name, PayRateTemplate source)
        {
            Name = name;
            SalaryPeriod = source.SalaryPeriod;
            BranchId = source.BranchId;
            PayRateTemplateDetails = source.PayRateTemplateDetails
                .Select(d => new PayRateTemplateDetail(Id, d.RuleType, d.RuleValue)).ToList();
        }

        public void AddOrUpdateRuleTemplateDetail(string ruleType, string ruleValue)
        {
            if (PayRateTemplateDetails == null) PayRateTemplateDetails = new List<PayRateTemplateDetail>();

            var existsTemplateDetail = PayRateTemplateDetails.FirstOrDefault(pd => pd.RuleType == ruleType);
            if (existsTemplateDetail == null)
            {
                PayRateTemplateDetails.Add(new PayRateTemplateDetail(Id, ruleType, ruleValue));
            }
            else
            {
                existsTemplateDetail.Update(ruleValue);
            }
        }

        public byte Status { get; set; }
    }
}
