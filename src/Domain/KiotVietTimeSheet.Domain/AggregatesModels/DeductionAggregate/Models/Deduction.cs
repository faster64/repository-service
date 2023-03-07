using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using System;

namespace KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models
{
    public class Deduction : BaseEntity,
        IEntityIdlong,
        ICreatedBy,
        ICreatedDate,
        IModifiedBy,
        IModifiedDate,
        ITenantId,
        ICode,
        ISoftDelete,
        IName
    {
        public static Deduction CreateInstance()
        {
            return new Deduction();
        }

        public const string CodePrefix = "GT";
        public const string CodeDelSuffix = "{DEL";

        // For EF
        private Deduction()
        { }

        public long Id { get; set; }
        public string Name { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int TenantId { get; set; }
        public string Code { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public long? DeletedBy { get; set; }
        public int? ValueType { get; set; }

        #region DeductionRuleValueDetail

        public decimal? Value { get; set; }
        public double? ValueRatio { get; set; }

        // trễ - sớm
        public int DeductionRuleId { get; set; }

        // phút - lần
        public int DeductionTypeId { get; set; }

        public int BlockTypeTimeValue { get; set; }
        public int BlockTypeMinuteValue { get; set; }

        #endregion DeductionRuleValueDetail

        public Deduction(string name)
        {
            Name = name;
        }

        public Deduction(string name, int valueType)
        {
            Name = name;
            ValueType = valueType;
        }

        public void SetDeductionRuleValueDetail(DeductionRuleValueDetail deductionDetail)
        {
            Value = ValueType == 1 ? deductionDetail.Value : null;
            ValueRatio = ValueType == 2 ? (double?)deductionDetail.Value : null;
            DeductionRuleId = deductionDetail.DeductionRuleId;
            DeductionTypeId = deductionDetail.DeductionTypeId;
            BlockTypeTimeValue = deductionDetail.BlockTypeTimeValue;
            BlockTypeMinuteValue = deductionDetail.BlockTypeMinuteValue;
        }

        public void Update(string name)
        {
            Name = name;
        }

        public void Update(string name, int valueType, DeductionRuleValueDetail deductionDetail)
        {
            Name = name;
            ValueType = valueType;
            SetDeductionRuleValueDetail(deductionDetail);
        }

        public void Delete()
        {
            IsDeleted = true;
        }
    }
}