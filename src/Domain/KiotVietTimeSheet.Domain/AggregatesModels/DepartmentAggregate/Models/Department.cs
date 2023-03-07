using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Events;
using KiotVietTimeSheet.SharedKernel.Extension;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models
{
    public class Department : BaseObjectEntity
    {
        #region Constructors
        public Department()
        {
            IsActive = true;
            IsDeleted = false;
        }

        public Department(string name, string description, bool isActive)
        {
            Name = name.ToPerfectString();
            Description = description.ToPerfectString();
            IsActive = isActive;

            AddDomainEvent(new CreatedDepartmentEvent(this));
        }

        // Only copy primitive values
        public Department(Department department)
        {
            Id = department.Id;
            Name = department.Name;
            Description = department.Description;
            IsActive = department.IsActive;
            TenantId = department.TenantId;
            CreatedBy = department.CreatedBy;
            CreatedDate = department.CreatedDate;
            ModifiedBy = department.ModifiedBy;
            ModifiedDate = department.ModifiedDate;
            IsDeleted = department.IsDeleted;
            DeletedBy = department.DeletedBy;
            DeletedDate = department.DeletedDate;
            if (department.DomainEvents != null)
            {
                foreach (var domainEvent in department.DomainEvents)
                {
                    AddDomainEvent(domainEvent);
                }
            }
        }

        #endregion

        #region Methods
        public void Update(string name, string description, bool isActive)
        {
            Name = name.ToPerfectString();
            Description = description.ToPerfectString();
            IsActive = isActive;

            AddDomainEvent(new UpdatedDepartmentEvent(this));
        }

        public void Delete()
        {
            IsDeleted = true;
            AddDomainEvent(new DeletedDepartmentEvent(this));
        }
        #endregion
    }
}
