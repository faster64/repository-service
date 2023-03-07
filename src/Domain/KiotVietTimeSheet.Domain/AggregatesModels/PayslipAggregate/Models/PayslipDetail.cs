using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models
{
    public class PayslipDetail : BaseEntity,
        IEntityIdlong,
        ITenantId
    {
        public long Id { get; set; }
        public long PayslipId { get; protected set; }
        public string RuleType { get; protected set; }
        public string RuleValue { get; protected set; }
        public string RuleParam { get; protected set; }
        public int TenantId { get; set; }

        #region Constructors
        [JsonConstructor]
        public PayslipDetail(
            long id,
            long payslipId,
            string ruleType,
            string ruleValue,
            string ruleParam,
            int tenantId
        )
        {
            Id = id;
            PayslipId = payslipId;
            RuleType = ruleType;
            RuleValue = ruleValue;
            RuleParam = ruleParam;
            TenantId = tenantId;
        }

        // Only copy primitive values
        public PayslipDetail(PayslipDetail payslipDetail)
        {
            Id = payslipDetail.Id;
            PayslipId = payslipDetail.PayslipId;
            RuleType = payslipDetail.RuleType;
            RuleValue = payslipDetail.RuleValue;
            RuleParam = payslipDetail.RuleParam;
            TenantId = payslipDetail.TenantId;
            if (payslipDetail.DomainEvents != null)
            {
                foreach (var domainEvent in payslipDetail.DomainEvents)
                {
                    AddDomainEvent(domainEvent);
                }
            }
        }

        public PayslipDetail(long payslipId, string ruleType, string ruleValue, string ruleParam)
        {
            PayslipId = payslipId;
            RuleType = ruleType;
            RuleValue = ruleValue;
            RuleParam = ruleParam;
        }
        #endregion

        public void UpdateRuleValue(string ruleValue)
        {
            RuleValue = ruleValue;
        }

        public void UpdateRuleParam(string ruleParam)
        {
            RuleParam = ruleParam;
        }
    }
}
