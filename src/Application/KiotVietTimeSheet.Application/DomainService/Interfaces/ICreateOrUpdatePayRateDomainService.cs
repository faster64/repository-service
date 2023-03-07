using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;

namespace KiotVietTimeSheet.Application.DomainService.Interfaces
{
    public interface ICreateOrUpdatePayRateDomainService
    {
        Task CreateAsync(PayRate payRate);
        Task UpdateAsync(PayRate payRate);
    }
}
