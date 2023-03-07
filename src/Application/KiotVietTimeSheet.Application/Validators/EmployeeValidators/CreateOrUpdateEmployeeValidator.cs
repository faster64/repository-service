using System;
using System.Collections.Generic;
using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Validations;
using Message = KiotVietTimeSheet.Resources.Message;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Domain.Utilities.Enums;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Specifications;
using KiotVietTimeSheet.Utilities;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;

namespace KiotVietTimeSheet.Application.Validators.EmployeeValidators
{
    public class CreateOrUpdateEmployeeValidator : BaseEmployeeValidator<Employee>
    {
        #region Properties
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;
        private readonly IAuthService _authService;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IPayRateTemplateReadOnlyRepository _payRateTemplateReadOnlyRepository;
        #endregion

        #region Constructors
        public CreateOrUpdateEmployeeValidator(
          IEmployeeWriteOnlyRepository employeeWriteOnlyRepository,
          IAuthService authService,
          IEmployeeReadOnlyRepository employeeReadOnlyRepository,
          ValidatorParamsObj validatorParamsObj
      )
        {
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
            _authService = authService;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            if (validatorParamsObj.IsSync)
            {
                ValidateBlockEmployee(validatorParamsObj.BlockUnit, validatorParamsObj.EmployeesPerBlock, retailId => _employeeWriteOnlyRepository.GetAllByWithoutPermission(retailId));
            }
            else
            {
                ValidateBlockEmployee(validatorParamsObj.BlockUnit, validatorParamsObj.EmployeesPerBlock,
                    retailerId =>
                        _employeeWriteOnlyRepository.GetBySpecificationAsync(
                            new FindEmployeeByTenantIdSpec(retailerId)));
            }

            ValidateCode();
            ValidateName();
            ValidateIdentityNumber();
            ValidateNote();
            ValidateProfilePictures();
            ValidatePhoneNumber();
            ValidateEmail();
            ValidateFacebook();
            ValidateAddress();
            ValidateMobilePhoneExist(validatorParamsObj.IsChangeMobileNumber);
            if (validatorParamsObj.IsSync)
            {
                ValidateCodeExist(validatorParamsObj.IsChangeCode, (retailerId, checkExistCodeSpec) => _employeeWriteOnlyRepository.AnyWithoutPermission(retailerId, checkExistCodeSpec));
            }
            else
            {
                ValidateCodeExist(validatorParamsObj.IsChangeCode, (_, checkExistCodeSpec) => _employeeWriteOnlyRepository.AnyBySpecificationAsync(checkExistCodeSpec));
            }

            if (validatorParamsObj.IsSync)
            {
                ValidateUserExist(validatorParamsObj.OldEmployeeIdOfUser, (retailId, findByUserId) => _employeeWriteOnlyRepository.GetAllByWithoutPermission(retailId, findByUserId));
            }
            else
            {
                ValidateUserExist(validatorParamsObj.OldEmployeeIdOfUser,
                    (_, findById) => _employeeWriteOnlyRepository.GetBySpecificationAsync(findById));
            }
        }
        public CreateOrUpdateEmployeeValidator(
            IEmployeeWriteOnlyRepository employeeWriteOnlyRepository,
            IPayRateTemplateReadOnlyRepository payRateTemplateReadOnlyRepository,
            IAuthService authService,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            ValidatorParamsObj validatorParamsObj
        )
        {
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
            _authService = authService;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _payRateTemplateReadOnlyRepository = payRateTemplateReadOnlyRepository;
            if (validatorParamsObj.IsSync)
            {
                ValidateBlockEmployee(validatorParamsObj.BlockUnit, validatorParamsObj.EmployeesPerBlock, retailId => _employeeWriteOnlyRepository.GetAllByWithoutPermission(retailId));
            }
            else
            {
                ValidateBlockEmployee(validatorParamsObj.BlockUnit, validatorParamsObj.EmployeesPerBlock,
                    retailerId =>
                        _employeeWriteOnlyRepository.GetBySpecificationAsync(
                            new FindEmployeeByTenantIdSpec(retailerId)));
            }

            ValidateCode();
            ValidateName();
            ValidateIdentityNumber();
            ValidateNote();
            ValidateProfilePictures();
            ValidatePhoneNumber();
            ValidateEmail();
            ValidateFacebook();
            ValidateAddress();
            ValidateMobilePhoneExist(validatorParamsObj.IsChangeMobileNumber);
            if (validatorParamsObj.IsSync)
            {
                ValidateCodeExist(validatorParamsObj.IsChangeCode, (retailerId, checkExistCodeSpec) => _employeeWriteOnlyRepository.AnyWithoutPermission(retailerId, checkExistCodeSpec));
            }
            else
            {
                ValidateCodeExist(validatorParamsObj.IsChangeCode, (_, checkExistCodeSpec) => _employeeWriteOnlyRepository.AnyBySpecificationAsync(checkExistCodeSpec));
            }

            if (validatorParamsObj.IsSync)
            {
                ValidateUserExist(validatorParamsObj.OldEmployeeIdOfUser, (retailId, findByUserId) => _employeeWriteOnlyRepository.GetAllByWithoutPermission(retailId, findByUserId));
            }
            else
            {
                ValidateUserExist(validatorParamsObj.OldEmployeeIdOfUser,
                    (_, findById) => _employeeWriteOnlyRepository.GetBySpecificationAsync(findById));
            }
            ValidatePayRateTemplate(validatorParamsObj.SourcePayRateTemplateId);
        }

        public CreateOrUpdateEmployeeValidator()
        {

        }
        #endregion

        #region Protected methods
        protected void ValidatePayRateTemplate(long sourcePayRateTemplateId)
        {
            RuleFor(e => e)
                .CustomAsync(async (employee, context, token) =>
                {
                    if (sourcePayRateTemplateId > 0)
                    {
                        var payRateTemplate =
                            await _payRateTemplateReadOnlyRepository.FindByIdAsync(sourcePayRateTemplateId);
                        if (payRateTemplate != null && payRateTemplate.Status == (byte)PayRateTemplateStatus.Deleted)
                        {
                            context.AddFailure(
                                string.Format(Message.delPayRateTemplateValidate, payRateTemplate.Name));
                        }
                    }
                });
        }

        protected void ValidateCodeExist(bool isChangeCode, Func<int, ISpecification<Employee>, Task<bool>> checkExistCodeFunc)
        {
            RuleFor(e => e)
                .MustAsync(async (employee, token) =>
                {
                    if (!string.IsNullOrEmpty(employee.Code) && (employee.Id == 0 || isChangeCode) && await checkExistCodeFunc(_authService.Context.TenantId, new CheckExistsCodeForOrmLiteSpec<Employee>(employee.Code)))
                    {
                        return false;
                    }
                    return true;
                })
                .WithMessage(string.Format(Message.is_existsInStore, Label.employee_code, ""));
        }

        protected void ValidateMobilePhoneExist(bool isChangeMobilePhone)
        {
            RuleFor(e => e)
                .MustAsync(async (employee, token) =>
                {
                    if (!string.IsNullOrEmpty(employee.MobilePhone) && (employee.Id == 0 || isChangeMobilePhone) && await _employeeReadOnlyRepository.CheckMobilePhoneExist(employee.Id, employee.MobilePhone))
                    {
                        return false;
                    }
                    return true;
                })
                .WithMessage(e => string.Format(Message.is_existsInStore, Label.mobilePhone, PhoneNumberHelper.StandardizePhoneNumber(e.MobilePhone, false)));
        }


        protected void ValidateUserExist(long? oldEmployeeIdOfUser,
            Func<int, ISpecification<Employee>, Task<List<Employee>>> getListEmployeeFunc)
        {
            RuleFor(e => e)
                .MustAsync(async (employee, token) =>
                {
                    if (employee.UserId != null && employee.UserId > 0)
                    {
                        var findEmployeeByUserIdSpec =
                            new FindEmployeeByUserIdSpec(employee.UserId).And(new NotEqualEmployeeIdSpec(employee.Id));

                        var employees =
                            await getListEmployeeFunc(_authService.Context.TenantId, findEmployeeByUserIdSpec);

                        if (employees.Any(x => x.Id != oldEmployeeIdOfUser))
                        {
                            return false;
                        }
                    }

                    return true;
                })
                .WithMessage(string.Format(Message.is_userHasMapEmployee, Label.employee));
        }

        protected void ValidateBlockEmployee(int? blockUnit, int? employeesPerBlock, Func<int, Task<List<Employee>>> getListFunc)
        {
            RuleFor(e => e)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .MustAsync(async (employee, context, token) =>
                {
                    if (employee == null) return true;

                    if (employeesPerBlock != null && employeesPerBlock > 0 && blockUnit != null && blockUnit > 0 && employee.Id == 0)
                    {
                        var employees = await getListFunc(_authService.Context.TenantId);
                        employees = employees.Where(x => !x.IsDeleted && x.IsActive).ToList();
                        if (employees.Count >= blockUnit * employeesPerBlock)
                        {
                            return false;
                        }
                    }
                    return true;

                }).WithErrorCode(ErrorCode.RunOutOfQuotaBlockEmployee.ToString()).WithMessage(string.Format(Message.not_allowRunningOutOfQuotaBlockEmployee, blockUnit * employeesPerBlock));
        }
        #endregion
    }
    public class ValidatorParamsObj
    {
        public ValidatorParamsObj(
            int? blockUnit,
            int employeesPerBlock = 0,
            bool isChangeCode = false,
            bool isChangeMobileNumber = false,
            long? oldEmployeeIdOfUser = null,
            long sourcePayRateTemplateId = 0,
            bool isSync = false)
        {
            BlockUnit = blockUnit;
            EmployeesPerBlock = employeesPerBlock;
            IsChangeCode = isChangeCode;
            IsChangeMobileNumber = isChangeMobileNumber;
            OldEmployeeIdOfUser = oldEmployeeIdOfUser;
            IsSync = isSync;
            SourcePayRateTemplateId = sourcePayRateTemplateId;
        }

        public int? BlockUnit { get; set; }
        public int EmployeesPerBlock { get; set; }
        public bool IsChangeCode { get; set; }
        public bool IsChangeMobileNumber { get; set; }
        public long? OldEmployeeIdOfUser { get; set; }
        public bool IsSync { get; set; }
        public long SourcePayRateTemplateId { get; set; }
    }

}
