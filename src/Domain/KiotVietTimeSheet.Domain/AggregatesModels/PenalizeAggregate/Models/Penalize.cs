

using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models
{
    public class Penalize : BaseEntity,
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
        public const string CodePrefix = "VP";
        public const string CodeDelSuffix = "{DEL";

        public static Penalize CreateInstance()
        {
            return new Penalize();
        }
        private Penalize() { }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public long? DeletedBy { get; set; }
        public List<ClockingPenalize> ClockingPenalizes { get; set; }

        public Penalize(string name)
        {
            Name = name;
        }

        public void Update(string name)
        {
            Name = name;
        }

        public Penalize CreateCopy(Penalize penalize)
        {
            return  new Penalize()
            {
                Id = penalize.Id,
                Name = penalize.Name,
                Code = penalize.Code,
                Value = penalize.Value,
                TenantId = penalize.TenantId,
                IsDeleted = penalize.IsDeleted,
                DeletedBy = penalize.DeletedBy,
                DeletedDate = penalize.DeletedDate,
                ModifiedBy = penalize.ModifiedBy,
                ModifiedDate = penalize.ModifiedDate,
                CreatedBy = penalize.CreatedBy,
                CreatedDate = penalize.CreatedDate
            };
        }

        public void Delete()
        {
            IsDeleted = true;
        }
    }
}
