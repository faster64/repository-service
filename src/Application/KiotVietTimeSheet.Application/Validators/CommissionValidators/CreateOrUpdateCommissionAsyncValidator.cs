using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Validators;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.CommissionValidators
{
    public class CreateOrUpdateCommissionAsyncValidator : BaseCommissionValidator<Commission>
    {
        #region PROPERTIES

        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IAuthService _authService;

        #endregion

        #region CONSTRUCTOR

        public CreateOrUpdateCommissionAsyncValidator(
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            Commission commission,
            IKiotVietServiceClient kiotVietServiceClient,
            IAuthService authService)
        {
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _kiotVietServiceClient = kiotVietServiceClient;
            _authService = authService;

            ValidateName();

            ValidateExistName(commission);

            ValidatePermission(commission);
        }

        #endregion

        #region METHODS

        protected void ValidatePermission(Commission commission)
        {
            if (commission.Id > 0) ValidCommissionIdGreaterThanZero();
            else ValidCommissionIdLessThanOrEqualZero();
        }

        private void ValidCommissionIdGreaterThanZero()
        {
            RuleFor(x => x)
                .MustAsync(async (c, token) =>
                {
                    if (_authService.Context.User.IsAdmin) return true;
                    if (c.CommissionBranches != null && c.CommissionBranches.Count > 0) return true;
                    if (c.IsAllBranch)
                    {
                        var commissionRepos = await _commissionReadOnlyRepository.FindByIdAsync(c.Id);
                        return (commissionRepos != null && commissionRepos.IsAllBranch);
                    }

                    var allBranch = (await _kiotVietServiceClient.GetBranch()).Data;
                    var branchHasPermissionCreate = (await _kiotVietServiceClient.GetBranchByPermission(TimeSheetPermission.Commission_Create)).Data;
                    return allBranch.All(cb => branchHasPermissionCreate.Any(bCreate => bCreate.Id == cb.Id));

                })
                .WithMessage(Message.commission_doNotHavePermissionToUpdate);
        }

        private void ValidCommissionIdLessThanOrEqualZero()
        {
            RuleFor(x => x)
                .MustAsync(async (c, token) =>
                {
                    if (_authService.Context.User.IsAdmin) return true;
                    var allBranch = (await _kiotVietServiceClient.GetBranch()).Data;
                    var branchHasPermissionCreate = (await _kiotVietServiceClient.GetBranchByPermission(TimeSheetPermission.Commission_Create)).Data;
                    if (c.IsAllBranch) return allBranch.Count == branchHasPermissionCreate.Count;
                    return c.CommissionBranches.All(cb => branchHasPermissionCreate.Any(bCreate => bCreate.Id == cb.BranchId));
                })
                .WithMessage(Message.commission_doNotHavePermissionToCreate);
        }

        /// <summary>
        /// Thực hiện validate form
        /// </summary>
        /// <param name="commission"></param>
        protected void ValidateExistName(Commission commission)
        {
            RuleFor(x => x)
                .MustAsync(async (c, token) => await CommissionNameIsAlreadyExists(commission))
                .WithMessage(Message.commission_existsName);
        }

        /// <summary>
        /// Kiểm tra tên bảng hoa hồng đã tồn tại hay chưa
        /// </summary>
        /// <param name="commission"></param>
        /// <returns></returns>
        private async Task<bool> CommissionNameIsAlreadyExists(Commission commission)
        {
            if (string.IsNullOrEmpty(commission.Name)) return true;
            return !(await _commissionReadOnlyRepository.AnyBySpecificationAsync(
                new FindCommissionByNameSpec(commission.Name).Not(new FindCommissionByIdSpec(commission.Id))));
        }

        #endregion
    }
}
