using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications;

namespace KiotVietTimeSheet.Application.DomainService.Impls
{
    public class DetectionChangePaysheetDomainService : IDetectionChangePaysheetDomainService
    {
        #region Properties
        private readonly IPaysheetReadOnlyRepository _paysheetReadOnlyRepository;
        #endregion
        public DetectionChangePaysheetDomainService(IPaysheetReadOnlyRepository paysheetReadOnlyRepository)
        {
            _paysheetReadOnlyRepository = paysheetReadOnlyRepository;
        }
        
        public async Task<bool> IsChangePaysheetWhenMakePaymentsAsync(long paysheetId, long paysheetVersion)
        {
            var existingPaysheet = await _paysheetReadOnlyRepository.FindBySpecificationAsync(new FindPaysheetByIdSpec(paysheetId));
            if (paysheetVersion != existingPaysheet.Version) return true;
            return false;
        }
    }
}
