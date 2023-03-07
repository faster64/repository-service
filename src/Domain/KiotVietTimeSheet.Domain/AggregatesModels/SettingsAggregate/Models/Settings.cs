using System;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Events;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models
{
    public class Settings : BaseEntity,
        IAggregateRoot,
        IEntityIdlong,
        ITenantId
    {
        public static Settings CreateInstance(int tenantId, string key, string value)
        {
            return new Settings(tenantId, key, value);
        }

        public static Settings Instance { get; } = new Settings();

        #region

        private Settings()
        {
            IsActive = false;
        }

        private Settings(int tenantId, string key, string value)
        {
            // create setting
            Name = key;
            TenantId = tenantId;
            IsActive = true;
            CreatedDate = DateTime.Now;
            Value = value;
            AddDomainEvent(new CreatedSettingsEvent(this));
        }
        public void Update(string value)
        {
            var oldSettings = (Settings)MemberwiseClone();
            Value = value;
            AddDomainEvent(new UpdateSettingsEvent(oldSettings, this));
        }
        // Only copy primitive values
        public Settings(Settings settings)
        {
            Id = settings.Id;
            TenantId = settings.TenantId;
            Name = settings.Name;
            Value = settings.Value;
            IsActive = settings.IsActive;
            CreatedDate = settings.CreatedDate;
            if (settings.DomainEvents != null)
            {
                foreach (var domainEvent in settings.DomainEvents)
                {
                    AddDomainEvent(domainEvent);
                }
            }
        }

        #endregion

        public long Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; protected set; }

        public string Value { get; set; }
        public bool IsActive { get; protected set; }
        public DateTime CreatedDate { get; protected set; }



    }
}
