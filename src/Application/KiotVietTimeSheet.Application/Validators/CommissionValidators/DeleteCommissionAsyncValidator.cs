using System.Linq;
using FluentValidation;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Validators;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.CommissionValidators
{
    public class DeleteCommissionAsyncValidator : BaseCommissionValidator<Commission>
    {
        #region PROPERTIES

        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IAuthService _authService;

        #endregion

        #region CONSTRUCTOR

        public DeleteCommissionAsyncValidator(
            Commission commission,
            IKiotVietServiceClient kiotVietServiceClient,
            IAuthService authService)
        {
            _kiotVietServiceClient = kiotVietServiceClient;
            _authService = authService;

            ValidatePermission(commission);
        }

        #endregion

        #region METHODS

        protected void ValidatePermission(Commission commission)
        {
            RuleFor(x => x)
                .MustAsync(async (c, token) =>
                {
                    if (_authService.Context.User.IsAdmin) return true;
                    var allBranch = (await _kiotVietServiceClient.GetBranch()).Data;
                    var branchHasPermissionDelete = (await _kiotVietServiceClient.GetBranchByPermission(TimeSheetPermission.Commission_Delete)).Data;
                    bool accessDenied;
                    if (c.IsAllBranch)
                    {
                        accessDenied = branchHasPermissionDelete.Count < allBranch.Count;
                        return !accessDenied;
                    }

                    accessDenied = !c.CommissionBranches.All(cb => branchHasPermissionDelete.Any(bDelete => bDelete.Id == cb.BranchId));

                    return !accessDenied;
                })
                .WithMessage(Message.commission_doNotHavePermissionToDelete);
        }

        #endregion
    }
}
