using System;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Dto
{
   public class SyncEmployeeDto    
   {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime? Dob { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string LocationName { get; set; }
        public string WardName { get; set; }
        public string Address { get; set; }
        public int TenantId { get; set; }
        public int BranchId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? UserId { get; set; }
        public bool IsActive { get; set; }
        public List<EmployeeBranchDto> EmployeeBranches { get; set; }

        public SyncEmployeeDto()
        {
            EmployeeBranches = new List<EmployeeBranchDto>();
        }
   }
}