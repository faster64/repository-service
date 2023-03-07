using FluentValidation;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Validations;
using Message = KiotVietTimeSheet.Resources.Message;
using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.EmployeeValidators
{
    public class DepartmentCreateOrUpdateValidator : BaseDepartmentValidator<Department>
    {
        #region Properties
        private readonly IDepartmentReadOnlyRepository _departmentReadOnlyRepository;
        #endregion

        #region Constructors
        public DepartmentCreateOrUpdateValidator(IDepartmentReadOnlyRepository departmentReadOnlyRepository)
        {
            _departmentReadOnlyRepository = departmentReadOnlyRepository;

            ValidateName();
            ValidateDescription();
            ValidateDepartmentIsExistsByNameAsync();
        }
        #endregion

        #region Protected methods
        protected void ValidateDepartmentIsExistsByNameAsync()
        {
            RuleFor(e => e)
                .MustAsync(async (department, token) =>
                {
                    var spec = (new FindDepartmentByNameSpec(department.Name)).Not(new FindDepartmentByIdSpec(department.Id));
                    var existingDepartment = await _departmentReadOnlyRepository.FindBySpecificationAsync(spec);

                    if (existingDepartment != null)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(string.Format(Message.is_existsInSystem, Label.department_name));
        }
        #endregion
    }
}