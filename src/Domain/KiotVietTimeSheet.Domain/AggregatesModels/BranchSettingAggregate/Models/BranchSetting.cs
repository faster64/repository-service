using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Events;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models
{
    public class BranchSetting : BaseEntity,
        IAggregateRoot,
        IEntityIdlong,
        IBranchId,
        ITenantId
    {
        #region PROPERTIES

        public long Id { get; set; }
        public int BranchId { get; set; }
        public string WorkingDays { get; protected set; }
        public int TenantId { get; set; }

        #endregion

        #region CONSTRUCTORS
        public BranchSetting() { }

        [JsonConstructor]
        public BranchSetting(long id, int branchId, string workingDays, int tenantId)
        {
            Id = id;
            BranchId = branchId;
            WorkingDays = workingDays;
            TenantId = tenantId;
        }

        // Only copy primitive values
        public BranchSetting(BranchSetting branchSetting)
        {
            Id = branchSetting.Id;
            BranchId = branchSetting.BranchId;
            WorkingDays = branchSetting.WorkingDays;
            TenantId = branchSetting.TenantId;
            if (branchSetting.DomainEvents != null)
            {
                foreach (var domainEvent in branchSetting.DomainEvents)
                {
                    AddDomainEvent(domainEvent);
                }
            }
        }
        public BranchSetting(int branchId, List<byte> workingDays)
        {
            BranchId = branchId;
            WorkingDays = string.Join(",", workingDays);

            AddDomainEvent(new CreatedBranchSettingEvent(this));
        }
        #endregion

        #region METHODS

        public static byte[] DefaultWorkingDays => new[]
        {
            (byte)DayOfWeek.Monday,
            (byte)DayOfWeek.Tuesday,
            (byte)DayOfWeek.Wednesday,
            (byte)DayOfWeek.Thursday,
            (byte)DayOfWeek.Friday,
            (byte)DayOfWeek.Saturday,
            (byte)DayOfWeek.Sunday
        };

        public void SetDefaultBranchSetting()
        {
            WorkingDays = string.Join(",", DefaultWorkingDays);
        }
        public void Update(List<byte> workingDays)
        {
            ChangeWorkingDays(workingDays);

            AddDomainEvent(new UpdatedBranchSettingEvent(this));
        }

        public void ChangeWorkingDays(List<byte> workingDays)
        {
            WorkingDays = string.Join(",", workingDays);
        }

        public byte[] WorkingDaysInArray
        {
            get
            {
                return string.IsNullOrEmpty(WorkingDays) ? Array.Empty<byte>() : GetWorkingDaysInArray();

#pragma warning disable S125 // Sections of code should not be commented out
                /*return (WorkingDays ?? string.Empty).Split(',').Select(d =>
                                {
                                    if (byte.TryParse(d, out var result))
                                    {
                                        return result;
                                    }

                                    return result;

                                }).ToArray();*/
            }
#pragma warning restore S125 // Sections of code should not be commented out
        }

        private byte[] GetWorkingDaysInArray()
        {
            const byte defaultResult = new byte();
            return (WorkingDays ?? string.Empty).Split(',').Select(d => byte.TryParse(d, out var result) ? result : defaultResult).ToArray();
        }

        #endregion
    }
}
