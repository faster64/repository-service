using System;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models
{
    public class CommissionDetail : BaseEntity,
        IAggregateRoot,
        IEntityIdlong,
        ITenantId,
        ICreatedBy,
        ICreatedDate,
        IModifiedBy,
        IModifiedDate,
        ISoftDelete,
        ICacheable
    {
        #region PROPERTIES
        public long Id { get; set; }
        public long CommissionId { get; set; }
        public long ObjectId { get; set; }
        public decimal? Value { get; set; }
        public decimal? ValueRatio { get; set; }
        public int TenantId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
        public byte Type { get; set; }
        #endregion

        #region Constructors

        public CommissionDetail() { }

        public CommissionDetail(long commissionId, long productId, decimal? value, decimal? valueRatio, byte type = (byte)CommissionDetailType.Product)
        {
            CommissionId = commissionId;
            ObjectId = productId;
            Value = value;
            ValueRatio = valueRatio;
            Type = type;
        }

        public CommissionDetail(long commissionId, long productId, int tennanId, decimal? value, decimal? valueRatio, byte type = (byte)CommissionDetailType.Product)
        {
            TenantId = tennanId;
            CommissionId = commissionId;
            ObjectId = productId;
            Value = value;
            ValueRatio = valueRatio;
            Type = type;
        }

        public CommissionDetail(int tenantId, long createdBy, long commissionId, long productId, decimal? value, decimal? valueRatio, byte type = (byte)CommissionDetailType.Product)
        {
            TenantId = tenantId;
            CreatedBy = createdBy;
            CommissionId = commissionId;
            ObjectId = productId;
            Value = value;
            ValueRatio = valueRatio;
            Type = type;
        }

        public CommissionDetail(long id, decimal? value)
        {
            Id = id;
            Value = value;
        }

        public void UpdateValue(decimal? value)
        {
            Value = value;
        }

        public void UpdateValueRatio(decimal? valueRatio)
        {
            ValueRatio = valueRatio;
        }

        public void Delete()
        {
            IsDeleted = true;
        }

        public CommissionDetail CreateCopy()
        {
            return new CommissionDetail
            {
                Id = Id,
                Value = Value,
                ValueRatio = ValueRatio,
                IsDeleted = IsDeleted
            };
        }

        public bool IsEqual(CommissionDetail commissionDetail)
        {
            if (commissionDetail == null) return false;
            return IsDeleted == commissionDetail.IsDeleted
                   && Value == commissionDetail.Value
                   && Type == commissionDetail.Type
                   && ValueRatio == commissionDetail.ValueRatio;
        }
        #endregion
    }
}
