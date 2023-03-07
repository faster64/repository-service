using System;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Extensions
{
    public static class ChangeTrackerExtensions
    {
        public static void SetShadowAuditProperties(this ChangeTracker changeTracker, IAuthService authService)
        {
            var timeStamp = DateTime.Now;

            foreach (var entry in changeTracker.Entries())
            {
                var entity = entry.Entity;

                switch (entry.State)
                {
                    case EntityState.Added:
                    {
                        SetTenantAndBranchAndCreatedByToEntity(authService, entity);
                        break;
                    }
                    case EntityState.Modified:
                    {
                        UpdateModifiedToEntity(authService, entity, timeStamp);
                        break;
                    }
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Deleted:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(changeTracker));
                }
            }
        }

        private static void UpdateModifiedToEntity(IAuthService authService, object entity, DateTime timeStamp)
        {
            if (entity is IModifiedDate auditRow)
                auditRow.ModifiedDate = timeStamp;

            if (entity is IModifiedBy modifyByRow)
                modifyByRow.ModifiedBy = authService.Context.User.Id;
        }

        private static void SetTenantAndBranchAndCreatedByToEntity(IAuthService authService, object entity)
        {
            if (entity is ITenantId tenantEntity)
                tenantEntity.TenantId = authService.Context.TenantId;

            if (entity is IBranchId branchEntity && branchEntity.BranchId == 0)
                branchEntity.BranchId = authService.Context.BranchId;

            if (entity is ICreatedBy createdByEntity)
                createdByEntity.CreatedBy = authService.Context.User.Id;
        }
    }
}
