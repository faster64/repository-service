using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.DomainService.Interfaces
{
    public interface IUpdateTenantNationalHolidayDomainService
    {
        Task UpdateTenantNationalHolidayAsync(int year);
    }
}
