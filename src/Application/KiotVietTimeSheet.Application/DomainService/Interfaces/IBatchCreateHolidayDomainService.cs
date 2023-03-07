using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;

namespace KiotVietTimeSheet.Application.DomainService.Interfaces
{
    public interface IBatchCreateHolidayDomainService
    {
        Task BatchCreateAutomaticAsync(List<Holiday> holidays);
    }
}
