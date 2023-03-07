using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models
{
    public class PayRateDetail : BaseEntity,
        IEntityIdlong,
        ITenantId
    {
        public long Id { get; set; }
        public long PayRateId { get; protected set; }
        public string RuleType { get; protected set; }
        public string RuleValue { get; protected set; }
        public int TenantId { get; set; }

        [JsonConstructor]
        public PayRateDetail(
            long id,
            int tenantId,
            long payRateId,
            string ruleType,
            string ruleValue
        )
        {
            Id = id;
            TenantId = tenantId;
            RuleType = ruleType;
            RuleValue = ruleValue;
            PayRateId = payRateId;
        }

        public PayRateDetail(string ruleType, string ruleValue)
        {
            RuleType = ruleType;
            RuleValue = ruleValue;
        }

        public PayRateDetail(long payRateId, string ruleType, string ruleValue, int tenantId)
        {
            PayRateId = payRateId;
            RuleType = ruleType;
            RuleValue = ruleValue;
            TenantId = tenantId;

        }

        public void Update(string ruleValue)
        {
            RuleValue = ruleValue;
        }
    }
}
