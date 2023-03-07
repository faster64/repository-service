using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Commands.CreateTenantNationHoliday
{
    public class CreateTenantNationHolidayCommand : BaseCommand<List<Holiday>>
    {
        public int TenantId { get; set; }

        public long UserId { get; set; }

        public int NationalHolidayCreationMonth { get; set; }

        public int NationalHolidayCreationDay { get; set; }

        public CreateTenantNationHolidayCommand(int tenantId, long userId, int nationalHolidayCreationMonth, int nationalHolidayCreationDay)
        {
            TenantId = tenantId;
            UserId = userId;
            NationalHolidayCreationMonth = nationalHolidayCreationMonth;
            NationalHolidayCreationDay = nationalHolidayCreationDay;
        }
    }
}
