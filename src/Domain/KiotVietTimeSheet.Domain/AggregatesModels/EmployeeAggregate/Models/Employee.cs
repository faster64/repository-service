using KiotVietTimeSheet.SharedKernel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models
{
    public class Employee : BaseEntity,
        IAggregateRoot,
        IEntityIdlong,
        ICode,
        IName,
        ITenantId,
        IBranchId,
        ICreatedBy,
        ICreatedDate,
        IModifiedBy,
        IModifiedDate,
        ISoftDelete,
        ICacheable
    {
        #region Properties
        public const string CodePrefix = "NV";
        public const string CodeDelSuffix = "{DEL";
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NickName  { get; set; } 
        public DateTime? DOB { get; protected set; }
        public bool? Gender { get; protected set; }
        public bool IsActive { get; protected set; }
        public string IdentityNumber { get; protected set; }
        public string MobilePhone { get; protected set; }
        public string Email { get; protected set; }
        public string Facebook { get; protected set; }
        public string Address { get; protected set; }
        public string LocationName { get; protected set; }
        public string WardName { get; protected set; }
        public string Note { get; protected set; }
        public long? UserId { get; protected set; }
        public long? DepartmentId { get; protected set; }
        public Department Department { get; set; }
        public long? JobTitleId { get; protected set; }
        public JobTitle JobTitle { get; set; }
        public string IdentityKeyClocking { get; set; }
        public string AccountSecretKey { get; set; }
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
        public string StandardizedMobilePhone { get; set; }
        public List<EmployeeProfilePicture> ProfilePictures { get; set; }
        public List<Clocking> Clockings { get; set; }

        public List<EmployeeBranch> EmployeeBranches { get; set; }
        #endregion

        #region Constructors
        // For EF

        public Employee()
        {
            IsActive = true;
            IsDeleted = false;
        }

        // Only copy primitive values
        public Employee(Employee employee)
        {
            Id = employee.Id;
            Code = employee.Code;
            Name = employee.Name;
            NickName = employee.NickName;
            DOB = employee.DOB;
            Gender = employee.Gender;
            IsActive = employee.IsActive;
            IdentityNumber = employee.IdentityNumber;
            MobilePhone = employee.MobilePhone;
            Email = employee.Email;
            Facebook = employee.Facebook;
            Address = employee.Address;
            LocationName = employee.LocationName;
            WardName = employee.WardName;
            Note = employee.Note;
            UserId = employee.UserId;
            DepartmentId = employee.DepartmentId;
            JobTitleId = employee.JobTitleId;
            TenantId = employee.TenantId;
            BranchId = employee.BranchId;
            CreatedBy = employee.CreatedBy;
            CreatedDate = employee.CreatedDate;
            ModifiedBy = employee.ModifiedBy;
            ModifiedDate = employee.ModifiedDate;
            IsDeleted = employee.IsDeleted;
            DeletedBy = employee.DeletedBy;
            DeletedDate = employee.DeletedDate;
            SecretKeyTakenUserId = employee.SecretKeyTakenUserId;
            if (employee.DomainEvents != null)
            {
                foreach (var domainEvent in employee.DomainEvents)
                {
                    AddDomainEvent(domainEvent);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tạo mới nhân viên
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="dob"></param>
        /// <param name="gender"></param>
        /// <param name="identityNumber"></param>
        /// <param name="mobilePhone"></param>
        /// <param name="email"></param>
        /// <param name="facebook"></param>
        /// <param name="address"></param>
        /// <param name="locationName"></param>
        /// <param name="wardName"></param>
        /// <param name="note"></param>
        /// <param name="departmentId"></param>
        /// <param name="jobTitleId"></param>
        /// <param name="profilePictures"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Employee(
            string code,
            string name,
            DateTime? dob,
            bool? gender,
            string identityNumber,
            string mobilePhone,
            string email,
            string facebook,
            string address,
            string locationName,
            string wardName,
            string note,
            long? departmentId,
            long? jobTitleId,
            List<EmployeeProfilePicture> profilePictures,
            long? userId,
            List<int> workBranchsList
        )
        {
            Code = code.ToPerfectString();
            Name = name.ToPerfectString();
            DOB = dob;
            Gender = gender;
            IsActive = true;
            IdentityNumber = identityNumber.ToPerfectString();
            MobilePhone = mobilePhone.ToPerfectString();
            Email = email.ToPerfectString();
            Facebook = facebook.ToPerfectString();
            Address = address.ToPerfectString();
            LocationName = locationName.ToPerfectString();
            WardName = wardName.ToPerfectString();
            Note = note.ToPerfectString();
            ProfilePictures = profilePictures;
            DepartmentId = departmentId;
            JobTitleId = jobTitleId;
            UserId = userId;
            EmployeeBranches = workBranchsList != null && workBranchsList.Count > 0
                ? workBranchsList.Select(branchId => new EmployeeBranch(branchId, 0)).ToList()
                : null;
        }

        /// <summary>
        /// Cập nhật nhân viên
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="dob"></param>
        /// <param name="gender"></param>
        /// <param name="identityNumber"></param>
        /// <param name="mobilePhone"></param>
        /// <param name="email"></param>
        /// <param name="facebook"></param>
        /// <param name="address"></param>
        /// <param name="locationName"></param>
        /// <param name="wardName"></param>
        /// <param name="note"></param>
        /// <param name="departmentId"></param>
        /// <param name="jobTitleId"></param>
        /// <param name="profilePictures"></param>
        /// <param name="userId"></param>
        public void Update(
            string code,
            string name,
            DateTime? dob,
            bool? gender,
            string identityNumber,
            string mobilePhone,
            string email,
            string facebook,
            string address,
            string locationName,
            string wardName,
            string note,
            long? departmentId,
            long? jobTitleId,
            List<EmployeeProfilePicture> profilePictures,
            long? userId,
            string identityKeyClocking
        )
        {
            Code = code.ToPerfectString();
            Name = name.ToPerfectString();
            DOB = dob;
            Gender = gender;
            IdentityNumber = identityNumber.ToPerfectString();
            MobilePhone = mobilePhone.ToPerfectString();
            Email = email.ToPerfectString();
            Facebook = facebook.ToPerfectString();
            Address = address.ToPerfectString();
            LocationName = locationName.ToPerfectString();
            WardName = wardName.ToPerfectString();
            Note = note.ToPerfectString();
            ProfilePictures = profilePictures.Select(s => new EmployeeProfilePicture(Id, s.ImageUrl, s.TenantId, s.IsMainImage)).ToList();
            DepartmentId = departmentId;
            JobTitleId = jobTitleId;
            UserId = userId;
            IdentityKeyClocking = identityKeyClocking;
        }

        public void UpdateUserId(long? userId)
        {
            UserId = userId;
        }

        public void UpdateProfilePicture()
        {
            ProfilePictures = null;
        }

        public void UnAssignUser()
        {
            UserId = null;
        }

        public void LeftDeparment()
        {
            DepartmentId = null;
        }

        public void LeftJobTitle()
        {
            JobTitleId = null;
        }

        public void Delete()
        {
            IsDeleted = true;
        }

        public void UpdateIdentityKeyClocking(string identityKeyClocking)
        {
            IdentityKeyClocking = identityKeyClocking;
        }
        #endregion
    }
}
