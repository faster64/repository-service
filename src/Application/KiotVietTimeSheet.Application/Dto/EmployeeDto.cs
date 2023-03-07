using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;

namespace KiotVietTimeSheet.Application.Dto
{
    public class EmployeeDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public DateTime? DOB { get; set; }
        public bool? Gender { get; set; }
        public bool IsActive { get; set; }
        public string IdentityNumber { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string Facebook { get; set; }
        public string Address { get; set; }
        public string LocationName { get; set; }
        public string WardName { get; set; }
        public string Note { get; set; }
        public long? UserId { get; set; }
        public long? DepartmentId { get; set; }
        public Department Department { get; set; }
        public long? JobTitleId { get; set; }
        public JobTitle JobTitle { get; set; }
        public string IdentityKeyClocking { get; set; }
        public int TenantId { get; set; }
        public int BranchId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public long? SecretKeyTakenUserId { get; set; }
        public List<EmployeeProfilePicture> ProfilePictures { get; set; }
        public List<ClockingDto> Clockings { get; set; }
        public string DepartmentName => Department?.Name ?? string.Empty;
        public string JobTitleName => JobTitle?.Name ?? string.Empty;
        public bool IsNotUpdateUserId { get; set; }
        public decimal Debt { get; set; }
        public UserDto User { get; set; }
        public List<string> FinqerCodes { get; set; }
        public List<int> WorkBranchIds { get; set; }
        public List<EmployeeBranch> EmployeeBranches { get; set; }
    }
}
