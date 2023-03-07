using System;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Extension;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Events;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models
{
    public class Commission : BaseEntity,
        IAggregateRoot,
        IEntityIdlong,
        ITenantId,
        ICreatedBy,
        ICreatedDate,
        IModifiedBy,
        IModifiedDate,
        ISoftDelete,
        ICacheable,
        IName
    {
        #region Properties
        public const string CodeDelSuffix = "{DEL";
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; protected set; }
        public int TenantId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsAllBranch { get; set; }
        public List<CommissionDetail> CommissionDetails { get; set; }
        public List<CommissionBranch> CommissionBranches { get; set; }
        #endregion

        #region Constructors
        public Commission()
        {
            IsActive = true;
            IsDeleted = false;
        }

        [JsonConstructor]
        public Commission(long id, string name, List<int> branchIds, bool isActive)
        {
            Id = id;
            Name = name;
            IsActive = isActive;
            CommissionBranches = branchIds != null && branchIds.Count > 0
                ? branchIds.Select(branchId => new CommissionBranch(branchId, 0)).ToList()
                : null;
        }


        #endregion

        #region Methods

        public Commission(string name, List<int> branchIds, bool isActive, bool isAllBranch)
        {
            Name = name;
            IsActive = isActive;
            IsAllBranch = isAllBranch;
            CommissionBranches = branchIds != null && branchIds.Count > 0
                ? branchIds.Select(branchId => new CommissionBranch(branchId, 0)).ToList()
                : null;

            AddDomainEvent(new CreatedCommissionEvent(this, branchIds));
        }

        public void Update(string name, List<int> branchIds, bool isActive, bool isAllBranch)
        {
            Name = name.ToPerfectString();
            IsActive = isActive;
            IsAllBranch = isAllBranch;
            CommissionBranches = branchIds != null && branchIds.Count > 0
                ? branchIds.Select(branchId => new CommissionBranch(branchId, 0)).ToList()
                : null;
            AddDomainEvent(new UpdatedCommissionEvent(this, branchIds));
        }

        public void Delete()
        {
            var beforeChange = MemberwiseClone() as Commission;

            IsDeleted = true;
            AddDomainEvent(new DeletedCommissionEvent(beforeChange));
        }

        #endregion
    }
}
