using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using ServiceStack;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2
{
    public class CommissionSalaryRuleValueV2 : IRuleValue
    {
        public CommissionSalaryTypes Type { get; set; }
        public CommissionSalaryFormalityTypes FormalityTypes { get; set; }
        public bool IsAllBranch { get; set; }
        public List<int> BranchIds { get; set; }
        public decimal? MinCommission { get; set; }
        public bool UseMinCommission { get; set; }
        public List<CommissionSalaryRuleValueDetailV2> CommissionSalaryRuleValueDetails { get; set; }

        public bool IsEqual(CommissionSalaryRuleValueV2 ruleValue)
        {
            if (ruleValue == null) return false;
            if (Type != ruleValue.Type) return false;
            if (FormalityTypes != ruleValue.FormalityTypes) return false;
            if (!IsEqualBranch(ruleValue.BranchIds, ruleValue.IsAllBranch)) return false;
            if (Type == CommissionSalaryTypes.WithMinimumCommission && MinCommission != ruleValue.MinCommission) return false;
            if (CommissionSalaryRuleValueDetails.IsNullOrEmpty() && !ruleValue.CommissionSalaryRuleValueDetails.IsNullOrEmpty()) return false;
            if (!CommissionSalaryRuleValueDetails.IsNullOrEmpty() && ruleValue.CommissionSalaryRuleValueDetails.IsNullOrEmpty()) return false;
            if (CommissionSalaryRuleValueDetails.Count != ruleValue.CommissionSalaryRuleValueDetails.Count) return false;
            return CommissionSalaryRuleValueDetails.TrueForAll(detail => ruleValue.CommissionSalaryRuleValueDetails.Any(detail.IsEqual));
        }

        private bool IsEqualBranch(List<int> ruleBranchIds, bool ruleIsAllBranch)
        {
            if (IsAllBranch != ruleIsAllBranch) return false;
            if (BranchIds.IsNullOrEmpty() && !ruleBranchIds.IsNullOrEmpty()) return false;
            if (!BranchIds.IsNullOrEmpty() && ruleBranchIds.IsNullOrEmpty()) return false;
            if (!BranchIds.IsNullOrEmpty() && !ruleBranchIds.IsNullOrEmpty() &&
                !BranchIds.SequenceEqual(ruleBranchIds)) return false;
            return true;
        }
    }
}
