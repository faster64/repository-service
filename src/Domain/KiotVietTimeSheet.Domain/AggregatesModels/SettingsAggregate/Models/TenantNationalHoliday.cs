using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models
{
    public class TenantNationalHoliday : BaseEntity,
        IEntityIdlong,
        ITenantId
    {
        public long Id { get; set; }
        public int TenantId { get; set; }
        public int LastCreatedYear { get; set; }

        public TenantNationalHoliday(int lastCreatedYear)
        {
            LastCreatedYear = lastCreatedYear;
        }

        public void UpdateLastCreatedYear(int lastCreatedYear)
        {
            LastCreatedYear = lastCreatedYear;
        }
    }
}
