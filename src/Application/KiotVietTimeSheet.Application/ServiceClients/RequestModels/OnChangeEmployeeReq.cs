using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.ServiceClients.RequestModels
{

    public class ChangeEmployeeBranchReq
    {
        public int RetailerId { get; set; }
        public int BranchId { get; set; }
        public long EmployeeId { get; set; }
    }

    public class ChangeEmployeeReq
    {
        public ChangeEmployeeReq()
        { }
        public ChangeEmployeeReq(EmployeeDto employeeDto, List<ChangeEmployeeBranchReq> employeeBranches) 
        {
            if(employeeDto!=null)
            {
                CreatedDate = employeeDto.CreatedDate;
                RetailerId = employeeDto.TenantId;
                Address = employeeDto.Address;
                BranchId = employeeDto.BranchId;
                DOB = employeeDto.DOB;
                Gender = employeeDto.Gender;
                LocationName = employeeDto.LocationName;
                MobilePhone = employeeDto.MobilePhone;
                Id = employeeDto.Id;
                Name = employeeDto.Name;
                WardName = employeeDto.WardName;
                UserId = employeeDto.UserId;
                Email = employeeDto.Email;
                IsDeleted = employeeDto.IsDeleted;
                Note = employeeDto.Note;
                ModifiedDate = employeeDto.ModifiedDate;
            } 
            EmployeeBranches = employeeBranches;
        }
        
        public DateTime CreatedDate { get; set; }
        public int BranchId { get; set; }
        public int RetailerId { get; set; }
        public string WardName { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public bool? Gender { get; set; }
        public string MobilePhone { get; set; }
        public DateTime? DOB { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
        public long? UserId { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
        public string Note { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<ChangeEmployeeBranchReq> EmployeeBranches { get; set; }
    }

    public class OnChangeEmployeeReq
    {
        public int RetailerId { get; set; }
        public ChangeEmployeeReq Employee { get; set; }        
    }

    public class OnDelEmployeeReq
    {
        public long RetailerId { get; set; }
        public long EmployeeId { get; set; }
    }
}
