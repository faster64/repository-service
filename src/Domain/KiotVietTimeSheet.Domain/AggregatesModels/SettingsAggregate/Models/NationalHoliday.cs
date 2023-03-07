using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models
{
    public class NationalHoliday : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte StartDay { get; set; }
        public byte EndDay { get; set; }
        public byte StartMonth { get; set; }
        public byte EndMonth { get; set; }
        public bool IsLunarCalendar { get; set; }
    }
}
