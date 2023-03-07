using System;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models
{
    public class PayRateTemplateDetail : BaseEntity,
        IAggregateRoot,
        IEntityIdlong,
        ITenantId,
        ICreatedBy,
        ICreatedDate,
        IModifiedBy,
        IModifiedDate
    {

        public long Id { get; set; }
        public int TenantId { get; set; }
        public long PayRateTemplateId { get; protected set; }
        public string RuleType { get; protected set; }
        public string RuleValue { get; protected set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        [JsonConstructor]
        public PayRateTemplateDetail(
            long id,
            int tenantId,
            long payRateTemplateId,
            string ruleType,
            string ruleValue,
            DateTime createdDate,
            long createdBy,
            long? modifiedBy,
            DateTime? modifiedDate
        )
        {
            Id = id;
            TenantId = tenantId;
            PayRateTemplateId = payRateTemplateId;
            RuleType = ruleType;
            RuleValue = ruleValue;
            CreatedDate = createdDate;
            ModifiedBy = modifiedBy;
            ModifiedDate = modifiedDate;
            CreatedBy = createdBy;
        }
        public PayRateTemplateDetail(long payRateTemplateId, string ruleType, string ruleValue)
        {
            PayRateTemplateId = payRateTemplateId;
            RuleType = ruleType;
            RuleValue = ruleValue;
        }

        public void Update(string ruleValue)
        {
            RuleValue = ruleValue;
        }
    }
}
