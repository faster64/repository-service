using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.EmployeeValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CreateDepartment
{
    public class CreateDepartmentCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateDepartmentCommand, DepartmentDto>
    {
        private readonly IMapper _mapper;
        private readonly DepartmentCreateOrUpdateValidator _departmentCreateOrUpdateValidator;
        private readonly IDepartmentWriteOnlyRepository _departmentWriteOnlyRepository;

        public CreateDepartmentCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IDepartmentWriteOnlyRepository departmentWriteOnlyRepository,
            DepartmentCreateOrUpdateValidator departmentCreateOrUpdateValidator
        )
            : base(eventDispatcher)
        {
            _mapper = mapper;
            _departmentWriteOnlyRepository = departmentWriteOnlyRepository;
            _departmentCreateOrUpdateValidator = departmentCreateOrUpdateValidator;
        }

        public async Task<DepartmentDto> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var departmentDto = request.Department;
            var department = new Department(departmentDto.Name, departmentDto.Description, departmentDto.IsActive);
            var validator = await _departmentCreateOrUpdateValidator.ValidateAsync(department);
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(Department), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return departmentDto;
            }

            _departmentWriteOnlyRepository.Add(department);

            await _departmentWriteOnlyRepository.UnitOfWork.CommitAsync();

            return _mapper.Map<DepartmentDto>(department);
        }
    }
}
