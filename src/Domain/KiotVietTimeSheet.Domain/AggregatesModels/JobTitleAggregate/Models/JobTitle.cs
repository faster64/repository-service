using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Events;
using KiotVietTimeSheet.SharedKernel.Extension;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models
{
    public class JobTitle : BaseObjectEntity
    {
        #region Constructors
        public JobTitle()
        {
            IsActive = true;
            IsDeleted = false;
        }

        public JobTitle(string name, string description, bool isActive)
        {
            Name = name.ToPerfectString();
            Description = description.ToPerfectString();
            IsActive = isActive;

            AddDomainEvent(new CreatedJobTitleEvent(this));
        }

        // Only copy primitive values
        public JobTitle(JobTitle jobTitle)
        {
            Id = jobTitle.Id;
            Name = jobTitle.Name;
            Description = jobTitle.Description;
            IsActive = jobTitle.IsActive;
            TenantId = jobTitle.TenantId;
            CreatedBy = jobTitle.CreatedBy;
            CreatedDate = jobTitle.CreatedDate;
            ModifiedBy = jobTitle.ModifiedBy;
            ModifiedDate = jobTitle.ModifiedDate;
            IsDeleted = jobTitle.IsDeleted;
            DeletedBy = jobTitle.DeletedBy;
            DeletedDate = jobTitle.DeletedDate;
            if (jobTitle.DomainEvents != null)
            {
                foreach (var domainEvent in jobTitle.DomainEvents)
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

            AddDomainEvent(new UpdatedJobTitleEvent(this));
        }

        public void Delete()
        {
            IsDeleted = true;
            AddDomainEvent(new DeletedJobTitleEvent(this));
        }
        #endregion
    }
}
