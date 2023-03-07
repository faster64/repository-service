using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.EmployeeValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateDepartmentCommand, Unit>
    {
        private readonly DepartmentCreateOrUpdateValidator _departmentCreateOrUpdateValidator;
        private readonly IDepartmentWriteOnlyRepository _departmentWriteOnlyRepository;

        public UpdateDepartmentCommandHandler(
            IEventDispatcher eventDispatcher,
            IDepartmentWriteOnlyRepository departmentWriteOnlyRepository,
            DepartmentCreateOrUpdateValidator departmentCreateOrUpdateValidator
        )
            : base(eventDispatcher)
        {
            _departmentWriteOnlyRepository = departmentWriteOnlyRepository;
            _departmentCreateOrUpdateValidator = departmentCreateOrUpdateValidator;
        }

        public async Task<Unit> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var departmentDto = request.Department;
            var existingDepartment = await _departmentWriteOnlyRepository.FindByIdAsync(departmentDto.Id);
            existingDepartment.Update(departmentDto.Name, departmentDto.Description, departmentDto.IsActive);

            var validator = await _departmentCreateOrUpdateValidator.ValidateAsync(existingDepartment, cancellationToken);
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(Department), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return Unit.Value;
            }

            _departmentWriteOnlyRepository.Update(existingDepartment);

            await _departmentWriteOnlyRepository.UnitOfWork.CommitAsync();
            return Unit.Value;
        }
    }
}
