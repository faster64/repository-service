using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Events;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models
{
    public class GpsInfo : BaseEntity,
        IAggregateRoot,
        IEntityIdlong,
        ITenantId,
        ICreatedBy,
        ICreatedDate,
        IModifiedBy,
        IModifiedDate,
        ISoftDelete,
        ICacheable,
        IBranchId
    {
        #region Properties
        public long Id { get; set; }
        public int TenantId { get; set; }
        public int BranchId { get; set; }
        public string Coordinate { get; set; }
        public string Address { get; set; }
        public string LocationName { get; set; }
        public string WardName { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public byte Status { get; set; }
        public string QrKey { get; set; }
        public int RadiusLimit { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }

        public List<ConfirmClocking> ConfirmClockings { get; set; }
        #endregion

        #region Constructors
        public GpsInfo()
        {
        }

        #endregion

        #region Methods

        public GpsInfo(int branchId, string coordinate, string address, string locationName, string wardName, string province, string district, byte status, string qrKey)
        {
            BranchId = branchId;
            Coordinate = coordinate;
            Address = address;
            LocationName = locationName;
            WardName = wardName;
            Province = province;
            District = district;
            Status = status;
            QrKey = qrKey;

            AddDomainEvent(new CreatedGpsInfoEvent(this));
        }

        public void Update(string coordinate, string address, string locationName, string wardName, string province, string district,int radiusLimit)
        {
            Coordinate = coordinate;
            Address = address;
            LocationName = locationName;
            WardName = wardName;
            Province = province;
            District = district;
            RadiusLimit = radiusLimit;
           

            AddDomainEvent(new UpdatedGpsInfoEvent(this));
        }

        public void Delete()
        {
            var beforeChange = MemberwiseClone() as GpsInfo;

            IsDeleted = true;
            AddDomainEvent(new DeletedGpsInfoEvent(beforeChange));
        }

        #endregion
    }
}
