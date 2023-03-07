using FluentValidation;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Resources;
using Message = KiotVietTimeSheet.Resources.Message;

namespace KiotVietTimeSheet.Application.Validators.EmployeeValidators
{
    public class AssignUserIdToEmployeeValidator : CreateOrUpdateEmployeeValidator
    {
        #region Properties
        #endregion
        public AssignUserIdToEmployeeValidator(
            IEmployeeWriteOnlyRepository employeeWriteOnlyRepository,
            IAuthService authService,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            int? blockUnit,
            long oldEmployeeIdOfUser,
            long? oldUserId = null,
            bool isChangeCode = false

            ) : base(employeeWriteOnlyRepository, authService, employeeReadOnlyRepository, new ValidatorParamsObj(blockUnit, 0, isChangeCode, true, oldEmployeeIdOfUser))
        {
            ValidateMappingEmployeeUser(oldUserId);
        }

        public AssignUserIdToEmployeeValidator(
            bool isCreateUser,
            long? oldUserId = null
        ) : base()
        {
            if (isCreateUser)
            {
                ValidateMappingEmployeeUserWhenCreateUser(oldUserId);
            }
            else
            {
                ValidateMappingEmployeeUser(oldUserId);
            }

        }
        #region Protected methods
        protected void ValidateMappingEmployeeUser(long? oldUserId)
        {
            RuleFor(e => e.UserId)
                .Must(userId =>
                {
                    if (oldUserId != null && userId != null && userId != oldUserId && userId > 0 && oldUserId > 0)
                    {
                        return false;
                    }
                    return true;
                })
                .WithMessage(string.Format(Message.is_hasMapUser, Label.employee));
        }
        protected void ValidateMappingEmployeeUserWhenCreateUser(long? oldUserId)
        {
            RuleFor(e => e.UserId)
                .Must(userId =>
                {
                    if (oldUserId != null && oldUserId != 0)
                    {
                        return false;
                    }
                    return true;
                })
                .WithMessage(string.Format(Message.is_hasMapUser, Label.employee));
        }
        #endregion
    }
}
