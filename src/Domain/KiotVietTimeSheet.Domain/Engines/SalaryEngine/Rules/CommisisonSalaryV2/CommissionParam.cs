using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.Domain.Common;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2
{
    public class CommissionParam
    {
        public byte CommissionType { get; set; }
        public decimal? CommissionLevel { get; set; }
        public decimal? Value { get; set; }
        public decimal? ValueRatio { get; set; }
        public decimal? ValueOrigin { get; set; }
        public decimal? ValueRatioOrigin { get; set; }
        public bool? IsDirty { get; set; }
        public List<ProductRevenue> ProductRevenues { get; set; }
        public CommissionTableParam CommissionTable { get; set; }
        public int CommissionSetting { get; set; }
    }

    public class CommissionTableParam
    {
        public CommissionTableParam() { }
        public CommissionTableParam(Commission commissionTable)
        {
            Id = commissionTable.Id;
            Name = commissionTable.Name;
            if (commissionTable.CommissionDetails != null && commissionTable.CommissionDetails.Any())
            {
                CommissionDetails = commissionTable.CommissionDetails
                    .Select(x => new CommissionTableDetailParam(x.Id, x.ObjectId, x.Value, x.ValueRatio, x.Type, x.IsDeleted))
                    .ToList();
            }
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public List<CommissionTableDetailParam> CommissionDetails { get; set; }
    }

    public class CommissionTableDetailParam
    {
        public CommissionTableDetailParam() { }
        public CommissionTableDetailParam(long id, long objectId, decimal? value, decimal? valueRatio, byte type, bool isDeleted)
        {
            Id = id;
            ObjectId = objectId;
            Value = ValueOrigin = value;
            ValueRatio = ValueRatioOrigin = valueRatio;
            IsDeleted = isDeleted;
            Type = type;
        }

        public long Id { get; set; }
        public long ObjectId { get; set; }
        public decimal? Value { get; set; }
        public decimal? ValueRatio { get; set; }
        public decimal? ValueOrigin { get; set; }
        public decimal? ValueRatioOrigin { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsDirty { get; set; }
        public byte Type { get; set; }
        public string BillCode { get; set; }
        public int TotalEmployee { get; set; }
        public string UniqueId { get; set; }
    }
}
