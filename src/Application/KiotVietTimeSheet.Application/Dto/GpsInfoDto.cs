using System;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Dto
{
    public class GpsInfoDto
    {
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

        public List<ConfirmClockingDto> ConfirmClockings { get; set; }
    }
}
