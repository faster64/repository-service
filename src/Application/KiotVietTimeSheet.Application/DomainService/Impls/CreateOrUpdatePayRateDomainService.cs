using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;

namespace KiotVietTimeSheet.Application.DomainService.Impls
{
    public class CreateOrUpdatePayRateDomainService : ICreateOrUpdatePayRateDomainService
    {
        #region Properties
        private readonly IPayRateWriteOnlyRepository _payRateWriteOnlyRepository;
        public IAuthService AuthService { get; }
        #endregion

        #region Constructors
        public CreateOrUpdatePayRateDomainService(
            IPayRateWriteOnlyRepository payRateWriteOnlyRepository,
            IAuthService authService
        )
        {
            _payRateWriteOnlyRepository = payRateWriteOnlyRepository;
            AuthService = authService;
        }
        #endregion

        #region Public methods

        public async Task CreateAsync(PayRate payRate)
        {
            if (await AuthService.HasPermissions(new string[] { TimeSheetPermission.PayRate_Update }))
            {
                _payRateWriteOnlyRepository.Add(payRate);
                await _payRateWriteOnlyRepository.UnitOfWork.CommitAsync();
            }
        }

        public async Task UpdateAsync(PayRate payRate)
        {
            if (await AuthService.HasPermissions(new string[] { TimeSheetPermission.PayRate_Update }))
            {
                await _payRateWriteOnlyRepository.UpdatePayRateAsync(payRate);
                await _payRateWriteOnlyRepository.UnitOfWork.CommitAsync();
            }
        }
        #endregion

        #region Public private
        #endregion
    }
}
