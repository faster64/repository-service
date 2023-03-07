using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models
{
    public class EmployeeProfilePicture : BaseEntity,
        IEntityIdlong,
        ITenantId
    {

        public long Id { get; set; }
        public string ImageUrl { get; set; }
        public bool IsMainImage { get; set; }
        public long EmployeeId { get; set; }
        public int TenantId { get; set; }
        public Employee Employee { get; set; }

        #region Constructor
        public EmployeeProfilePicture()
        {
            IsMainImage = false;
        }
        public EmployeeProfilePicture(long employeeId, string imageUrl, int tenantId, bool isMainImage = false)
        {
            EmployeeId = employeeId;
            ImageUrl = imageUrl;
            IsMainImage = isMainImage;
            TenantId = tenantId;
        }

        // Only copy primitive values
        public EmployeeProfilePicture(EmployeeProfilePicture employeeProfilePicture)
        {
            Id = employeeProfilePicture.Id;
            ImageUrl = employeeProfilePicture.ImageUrl;
            IsMainImage = employeeProfilePicture.IsMainImage;
            EmployeeId = employeeProfilePicture.EmployeeId;
            TenantId = employeeProfilePicture.TenantId;
            if (employeeProfilePicture.DomainEvents != null)
            {
                foreach (var domainEvent in employeeProfilePicture.DomainEvents)
                {
                    AddDomainEvent(domainEvent);
                }
            }
        }

        #endregion
    }
}
