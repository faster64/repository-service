using System;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models
{
    public class Allowance : BaseEntity,
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
        public static Allowance Instance { get; } = new Allowance();
        public const string CodePrefix = "PC";
        public const string CodeDelSuffix = "{DEL";

        // For EF
        private Allowance() { }

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
        public double Value { get; set; }
        public double ValueRatio { get; set; }
        public bool IsChecked { get; set; }
        public int? Type { get; set; }
        public double Rank { get; set; }
        public Allowance(string name)
        {
            Name = name;
        }

        public void Update(string name,int? type, bool isChecked, double valueRatio, double value, double rank)
        {
            Name = name;
            Type = type;
            IsChecked = isChecked;
            Value = value;
            ValueRatio = valueRatio;
            Rank = rank;
        }

        public void Delete()
        {
            IsDeleted = true;
        }
    }
}
